using PngDecoder.Exceptions;
using PngDecoder.Extension;
using PngDecoder.Models;
using PngDecoder.Models.ColorReader;
using PngDecoder.Models.Filters;
using System.Buffers;
using System.Diagnostics;
using System.IO.Compression;

namespace PngDecoder;

public class PNGDecode
{
    private readonly Stream _fileStream;
    private List<PNGChunk> _chunks;

    private static ReadOnlySpan<byte> headerSignature =>
        [137, 80, 78, 71, 13, 10, 26, 10];

    public PNGDecode(Stream fileStream)
    {
        _chunks = new List<PNGChunk>(4);
        _fileStream = fileStream;
        Span<byte> signature = stackalloc byte[headerSignature.Length];
        _fileStream.Read(signature);
        if (!GenericHelper.Equal(signature, headerSignature))
            throw new SignatureException(signature.ToArray());

        while (_fileStream.Position < _fileStream.Length)
        {
            var chunk = new PNGChunk(_fileStream);
            _chunks.Add(chunk);
            if (chunk.Signature == PngChunkType.IEND)
                break;
        }

        //check essentials
        // IEND, IDAT, IHDR
    }
    public int Height { get => (int)new IHDRData(_chunks.First(a => a.Signature == PngChunkType.IHDR)).Height; }
    public int width { get => (int)new IHDRData(_chunks.First(a => a.Signature == PngChunkType.IHDR)).Width; }

    public byte[] DecodeImageData()
    {
        var header = _chunks.First(a => a.Signature == PngChunkType.IHDR);
        var headerData = new IHDRData(header);

        var paletteData = new PLTEData?();

        if (headerData.ColorType == ColorType.Palette)
        {
            var palate = _chunks.First(a => a.Signature == PngChunkType.PLTE);
            paletteData = new PLTEData(palate);
        }
        var colorConverter = GetColorConverter(headerData, paletteData);

            var writtenIndex = 0;
            var currentRow = -1;
            var result = new byte[headerData.Height * headerData.Width * 4];
            using var rawstream = GetFilteredRawStream2();
            using var filteredMutableRawStream = new MemoryStream();
            rawstream.CopyTo(filteredMutableRawStream);
            UnfilterStream(filteredMutableRawStream, colorConverter, result, ref writtenIndex, ref currentRow);
            return result;
        }

    // TODO write own ZLib to minimize foot-print even more
    {
        var result = new MemoryStream();
        foreach (var chunk in _chunks.Where(a => a.Signature == PngChunkType.IDAT))
        {
            var data = ArrayPool<byte>.Shared.Rent((int)chunk.Length);
            try
            {
                chunk.GetData(data);
                result.Write(data, 0, (int)chunk.Length);
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(data);
            }
        }
        result.Position = 0;
        return new ZLibStream(result, CompressionMode.Decompress, false);
    }


    private void UnfilterStream(Stream filteredRawData, BaseRGBColorConverter converter, byte[] result, ref int writtenIndex, ref int currentRow)
    {
        filteredRawData.Seek(0, SeekOrigin.Begin);
        Span<byte> currentByte = stackalloc byte[1];
        var writtenSection = new Span<byte>();
        var scanlineLength = converter.Ihdr.GetScanLinesWidthWithPadding() + 1;
        BaseFilter filter = new NonFilter(filteredRawData);
        while (filteredRawData.Read(currentByte) != 0)
        {
            if (filteredRawData.Position == 1 || filteredRawData.Position % scanlineLength == 1)
            {
                writtenIndex = 0;
                currentRow++;
                filter = GetFilter(currentByte[0], filteredRawData, converter.Ihdr.GetPixelSizeInByte());
                writtenSection = new Span<byte>(result,
                    (int)(currentRow * converter.Ihdr.Width * 4),
                    (int)converter.Ihdr.Width * 4);
                continue;
            }
            //TODO: can be do prcess the number requied pixels or a full pixel.
            var compressByte = filter.UnApply(currentByte[0], scanlineLength);
            converter.Write(writtenSection, compressByte, ref writtenIndex);
        }
    }

    private static BaseFilter GetFilter(byte mode, Stream filteredRawData, byte pixelSize) =>
        mode switch
        {
            0 => new NonFilter(filteredRawData),
            1 => new SubFilter(filteredRawData, pixelSize),
            2 => new UpFilter(filteredRawData),
            3 => new AverageFilter(filteredRawData, pixelSize),
            4 => new PaethFilter(filteredRawData, pixelSize),
            _ => throw new NotImplementedException()
        };

    private static BaseRGBColorConverter GetColorConverter(IHDRData ihdr, PLTEData? plte) =>
        ihdr.ColorType switch
        {
            ColorType.Palette => new PalateColorConverter(plte!.Value, ihdr),
            ColorType.GreyScale => new GrayScaleColorConverter(ihdr),
            ColorType.RGB => new RGBColorConverter(ihdr),
            ColorType.GreyScaleAndAlpha => new GreyScaleAndAlphaConverter(ihdr),
            ColorType.RGBA => new RGBAColorConverter(ihdr),
            _ => throw new NotSupportedException(),
        };

}
