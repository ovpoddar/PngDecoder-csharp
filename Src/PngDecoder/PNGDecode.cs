using PngDecoder.Exceptions;
using PngDecoder.Extension;
using PngDecoder.Models;
using PngDecoder.Models.ColorReader;
using PngDecoder.Models.Filters;
using System;
using System.Buffers;
using System.ComponentModel.DataAnnotations;
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
        var writingIndex = 0;
        var result = new byte[headerData.Height * headerData.Width * 4];
        using var rawstream = GetFilteredRawStream2();
        using var filteredMutableRawStream = new MemoryStream();
        rawstream.CopyTo(filteredMutableRawStream);

        BaseRGBColorConverter colorConverter = GetPixelColorDecoder(ref headerData, ref paletteData);
        DecodeZLibStream(filteredMutableRawStream, ref headerData, result, ref writingIndex, colorConverter);
        return result;
    }

    private void DecodeZLibStream(MemoryStream filteredMutableRawStream,
        ref IHDRData headerData, 
        byte[] result,
        ref int writingIndex,
        BaseRGBColorConverter colorConverter)
    {
        filteredMutableRawStream.Position = 0;
        var scanline = headerData.GetScanLinesWidthWithPadding();
        BasePNGFilter? currentFilter = null;
        var writtenSection = new Span<byte>();
        var currentRow = 0;
        while (filteredMutableRawStream.Length > filteredMutableRawStream.Position)
        {
            if (filteredMutableRawStream.Position % scanline == 0)
            {
                // Filter
                currentFilter = GetFilter(filteredMutableRawStream);
                writtenSection = new Span<byte>(result,
                    (int)(currentRow * headerData.Width * 4),
                    (int)headerData.Width * 4);
                currentRow++;
                writingIndex = 0;
            }
            else
            {
                ReadImage(filteredMutableRawStream,
                    ref currentFilter!,
                    ref headerData, 
                    ref writingIndex, 
                    colorConverter,
                    writtenSection);
            }
        }
    }

    private BaseRGBColorConverter GetPixelColorDecoder(ref IHDRData headerData, ref PLTEData? paletteData) =>
        headerData.ColorType switch
        {
            ColorType.Palette => new PalateColorConverter(paletteData!.Value, headerData),
            //ColorType.GreyScale => new GrayScaleColorConverter(ihdr),
            //ColorType.RGB => new RGBColorConverter(ihdr),
            //ColorType.GreyScaleAndAlpha => new GreyScaleAndAlphaConverter(ihdr),
            //ColorType.RGBA => new RGBAColorConverter(ihdr),
            _ => throw new NotSupportedException(),
        };

    // TODO write own ZLib to minimize foot-print even more
    ZLibStream GetFilteredRawStream2()
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


    BasePNGFilter GetFilter(Stream stream)
    {
        Span<byte> currentByte = stackalloc byte[1];
        var totalRead = stream.Read(currentByte);
#if DEBUG
        if (totalRead != 1) throw new Exception("Big issue");
#endif
        return currentByte[0] switch
        {
            0 => new NonFilter(stream),
            1 => new SubFilter(stream),
            2 => new UpFilter(stream),
            3 => new AverageFilter(stream),
            4 => new PaethFilter(stream),
            _ => throw new NotImplementedException()
        };
    }

    void ReadImage(Stream stream, 
        ref BasePNGFilter currentFilter, 
        ref IHDRData iHDR, 
        ref int writingIndex, 
        BaseRGBColorConverter colorConverter,
        Span<byte> result)
    {
        Span<byte> currentByte = iHDR.BitDepth > 8
            ? stackalloc byte[1]
            : stackalloc byte[iHDR.GetPixelSizeInByte()];
        var totalRead = stream.Read(currentByte);
#if DEBUG
        if (totalRead != currentByte.Length) throw new Exception("Big issue");
#endif
        currentByte = currentFilter.UnApply(currentByte, iHDR.GetScanLinesWidthWithPadding());
        colorConverter.Write(result, currentByte, ref writingIndex);
    }
}
