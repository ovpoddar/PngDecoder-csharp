using PngDecoder.Exceptions;
using PngDecoder.Extension;
using PngDecoder.Models;
using PngDecoder.Models.ColorReader;
using PngDecoder.Models.Filters;
using System;
using System.Buffers;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;

namespace PngDecoder;

public class PNGDecode
{
    private readonly Stream _fileStream;
    private readonly List<PNGChunk> _chunks;

    private static byte[] HeaderSignature =>
        [137, 80, 78, 71, 13, 10, 26, 10];

    public PNGDecode(Stream fileStream)
    {
        _chunks = new List<PNGChunk>(4);
        _fileStream = fileStream;
        Span<byte> signature = stackalloc byte[HeaderSignature.Length];
        _fileStream.Read(signature);
        if (!GenericHelper.Equal(signature, HeaderSignature))
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
        var processRow = 0;
        var result = new byte[headerData.Height * headerData.Width * 4];
        var colorConverter = GetPixelColorDecoder(ref headerData, ref paletteData);

        using var rawstream = GetDecompressedRawStream();
        using var filteredMutableRawStream = new MemoryStream();
        rawstream.CopyTo(filteredMutableRawStream);
        DecodeZLibStream(filteredMutableRawStream, ref headerData, result, ref writingIndex, ref processRow, colorConverter);
        return result;
    }

    private void DecodeZLibStream(MemoryStream filteredMutableRawStream,
        ref IHDRData headerData,
        byte[] result,
        ref int writingIndex,
        ref int currentRow,
        BaseRGBColorConverter colorConverter)
    {
        filteredMutableRawStream.Position = 0;
        var scanline = headerData.GetScanLinesWidthWithPadding();
        BasePNGFilter? currentFilter = null;
        Span<byte> writtenSection = new();
        Span<byte> currentByte = headerData.BitDepth < 8
            ? stackalloc byte[1]
            : stackalloc byte[headerData.GetPixelSizeInByte()];
        Span<byte> currentFilterByte = stackalloc byte[1];

        while (filteredMutableRawStream.Position < filteredMutableRawStream.Length)
        {
            if (filteredMutableRawStream.Position % scanline == 0)
            {
                filteredMutableRawStream.ReadExactly(currentFilterByte);
                currentFilter = GetFilter(filteredMutableRawStream, currentFilterByte[0]);
                writtenSection = new Span<byte>(result, (int)(currentRow++ * headerData.Width * 4), (int)headerData.Width * 4);
                writingIndex = 0;
            }
            else
            {
                filteredMutableRawStream.ReadExactly(currentByte);
                currentByte = currentFilter!.UnApply(currentByte, scanline);
                colorConverter.Write(writtenSection, currentByte, ref writingIndex);
            }
        }
    }

    static BaseRGBColorConverter GetPixelColorDecoder(ref IHDRData headerData, ref PLTEData? paletteData) =>
        headerData.ColorType switch
        {
            ColorType.Palette => new PalateColorConverter(paletteData!.Value, headerData),
            ColorType.GreyScale => new GrayScaleColorConverter(headerData),
            ColorType.RGB => new RGBColorConverter(headerData),
            ColorType.GreyScaleAndAlpha => new GreyScaleAndAlphaConverter(headerData),
            ColorType.RGBA => new RGBAColorConverter(headerData),
            _ => throw new NotSupportedException(),
        };

    // TODO write own ZLib to minimize foot-print even more
    ZLibStream GetDecompressedRawStream()
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

    static BasePNGFilter GetFilter(Stream stream, byte currentByte) =>
        currentByte switch
        {
            0 => new NonFilter(stream),
            1 => new SubFilter(stream),
            2 => new UpFilter(stream),
            3 => new AverageFilter(stream),
            4 => new PaethFilter(stream),
            _ => throw new NotImplementedException()
        };
}
