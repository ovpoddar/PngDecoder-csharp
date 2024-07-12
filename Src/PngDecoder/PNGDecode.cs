﻿using PngDecoder.Exceptions;
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

        while (true || _fileStream.Position != _fileStream.Length)
        {
            var chunk = new PNGChunk(_fileStream);
            _chunks.Add(chunk);
            if (chunk.Signature == PngChunkType.IEND)
                break;
        }

        //check essentials
        // IEND, IDAT, IHDR
    }

    public byte[] DecodeImageData()
    {
        var header = _chunks.First(a => a.Signature == PngChunkType.IHDR);
        var headerData = new IHDRData(header);

        var result = new byte[headerData.Height * headerData.Width * 4];
        var scanlineLength = headerData.GetScanLinesWidthWithPadding();
        var paletteData = new PLTEData?();

        if (headerData.ColorType == ColorType.Palette)
        {
            var palate = _chunks.First(a => a.Signature == PngChunkType.PLTE);
            paletteData = new PLTEData(palate);
        }
        var colorConverter = GetColorConverter(headerData, paletteData);
        using var encodedFilteredRawData = new MemoryStream();
        ProcessIDATChunks(encodedFilteredRawData);
        // need my own zlib stream looks like just skipping 6 byte its second one %32 ==0
        using var filteredRawStream = new ZLibStream(encodedFilteredRawData, CompressionMode.Decompress, false);
        using var filteredMutableRawStream = new MemoryStream();
        filteredRawStream.CopyTo(filteredMutableRawStream);
        UnfilterStream(filteredMutableRawStream, colorConverter, result, ++scanlineLength);
        return result;
    }

    private void ProcessIDATChunks(Stream raw)
    {
        foreach (var chunk in _chunks.Where(a => a.Signature == PngChunkType.IDAT))
        {
            var data = ArrayPool<byte>.Shared.Rent((int)chunk.Length);
            chunk.GetData(data);
            try
            {
                raw.Write(data);
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(data);
            }
            break;
        }
        raw.Position = 0;
    }

    private void UnfilterStream(Stream filteredRawData, BaseRGBColorConverter converter, byte[] result, int scanLineLength)
    {
        filteredRawData.Seek(0, SeekOrigin.Begin);
        var writtenIndex = 0;
        var currentRow = -1;
        Span<byte> currentByte = stackalloc byte[1];
        var writtenSection = new Span<byte>();
        BaseFilter filter = new NonFilter(filteredRawData);
        while (filteredRawData.Read(currentByte) != 0)
        {
            if (filteredRawData.Position == 1 || filteredRawData.Position % scanLineLength == 1)
            {
                writtenIndex = 0;
                currentRow++;
                filter = GetFilter(currentByte[0], filteredRawData, converter.Ihdr.GetPixelSizeInByte());
                writtenSection = new Span<byte>(result,
                    (int)(currentRow * converter.Ihdr.Width * 4),
                    (int)converter.Ihdr.Width * 4);
                continue;
            }

            // need to test more 16 bit
            var compressByte = filter.UnApply(currentByte[0], scanLineLength);
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
            _ => throw new NotSupportedException(),
        };

    public bool Check() =>
        _chunks.Any();

}
