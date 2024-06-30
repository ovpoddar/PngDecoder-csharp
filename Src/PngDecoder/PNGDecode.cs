using PngDecoder.Exceptions;
using PngDecoder.Extension;
using PngDecoder.Filters;
using PngDecoder.Models;
using System.Buffers;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Reflection.PortableExecutable;

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
        var scanlineLength = headerData.GetScanLinesWidth();
        var paletteData = new PLTEData?();

        if (headerData.ColorType == ColorType.Palette)
        {
            var palate = _chunks.First(a => a.Signature == PngChunkType.PLTE);
            paletteData = new PLTEData(palate);
        }

        var data = _chunks.First(a => a.Signature == PngChunkType.IDAT);
        var raw = ArrayPool<byte>.Shared.Rent((int)data.Length);
        data.GetData(raw);
        try
        {
            using var encodedFilteredRawData = new MemoryStream(raw);
            using var filteredRawStream = new ZLibStream(encodedFilteredRawData, CompressionMode.Decompress, false);
            using var filteredMutableRawStream = new MemoryStream();
            filteredRawStream.CopyTo(filteredRawStream);
            UnfilterStream(filteredMutableRawStream, ++scanlineLength);
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(raw);
        }

        return null;
    }

    private void UnfilterStream(Stream filteredRawData, int scanLineLength)
    {
        filteredRawData.Seek(0, SeekOrigin.Begin);
        Span<byte> currentByte = stackalloc byte[1];
        BaseFilter filter = new NonFilter();
        while (filteredRawData.Read(currentByte) != 0)
        {
            if (filteredRawData.Position == 1 || filteredRawData.Position % scanLineLength == 1)
            {
                filter = GetFilter(currentByte[0]);
                continue;
            }
            filter.Apply(filteredRawData, currentByte[0], scanLineLength);
        }
    }

    private BaseFilter GetFilter(byte mode) =>
        mode switch
        {
            0 => new NonFilter(),
            1 => new SubFilter(),
            _ => null
        };

    public bool Check() =>
        _chunks.Any();

}
