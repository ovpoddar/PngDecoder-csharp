// Ignore Spelling: CRC

using PngDecoder.Extension;

namespace PngDecoder.Models;
internal struct PNGChunk
{
    private readonly Stream _stream;

    public uint Length { get; set; }
    public PngChunkType Signature { get; set; }
    public long Data { get; set; }
    public long CRC { get; set; }

    public PNGChunk(Stream stream)
    {
        _stream = stream;

        Span<byte> responce = stackalloc byte[4];
        stream.Read(responce);
        MemoryExtensions.Reverse(responce);
        this.Length = responce.ToStruct<uint>();

        stream.Read(responce);
        this.Signature = responce.ToStruct<PngChunkType>();

        this.Data = stream.Position;
        _stream.Seek(this.Length, SeekOrigin.Current);

        this.CRC = stream.Position;
        _stream.Seek(4, SeekOrigin.Current);
    }

    public void GetData(Span<byte> result)
    {
        var oldPosation = _stream.Position;
        _stream.Seek(Data, SeekOrigin.Begin);
        _stream.Read(result);
        _stream.Seek(oldPosation, SeekOrigin.Begin);
    }

    public byte[] GetData()
    {
        var oldPosation = _stream.Position;
        var result = new byte[this.Length];
        _stream.Seek(Data, SeekOrigin.Begin);
        _stream.Read(result, 0, result.Length);
        _stream.Seek(oldPosation, SeekOrigin.Begin);
        return result;
    }

    public void GetData(byte[] result)
    {
        var oldPosation = _stream.Position;
        _stream.Seek(Data, SeekOrigin.Begin);
        _stream.Read(result, 0, (int)Length);
        _stream.Seek(oldPosation, SeekOrigin.Begin);
    }

    public byte[] GetCRC()
    {
        var oldPosation = _stream.Position;
        Span<byte> result = stackalloc byte[4];
        _stream.Seek(Data, SeekOrigin.Begin);
        _stream.Read(result);
        _stream.Seek(oldPosation, SeekOrigin.Begin);
        return result.ToArray();
    }
}