namespace PngDecoder.Models.Filters;
public class BaseFilter
{
    private readonly Stream _stream;

    public BaseFilter(Stream stream) =>
        _stream = stream;

    public virtual byte UnApply(byte current, int scanLineWidth)
    {
        _stream.Seek(-1, SeekOrigin.Current);
        _stream.WriteByte(current);
        return current;
    }

    public byte GetLeftByte(int lineWidth)
    {
        Span<byte> result = stackalloc byte[1];
        if (_stream.Position % lineWidth != 2)
        {
            _stream.Seek(-2, SeekOrigin.Current);
            _stream.Read(result);
            _stream.Seek(1, SeekOrigin.Current);
        }
        return result[0];
    }

    public byte GetTopByte(int lineWidth)
    {
        var topIndex = _stream.Position - lineWidth - 1;
        Span<byte> result = stackalloc byte[1];
        if (topIndex >= 0)
        {
            var tempCurrentIndex = _stream.Position;
            _stream.Seek(topIndex, SeekOrigin.Begin);
            _stream.Read(result);

            _stream.Seek(tempCurrentIndex, SeekOrigin.Begin);
        }
        return result[0];
    }

    public byte GetTopLeftByte(int lineWidth)
    {
        var topLeftIndex = _stream.Position - lineWidth - 2;
        Span<byte> result = stackalloc byte[1];
        if (topLeftIndex >= 0 && _stream.Position % lineWidth != 2)
        {
            var tempCurrentIndex = _stream.Position;

            _stream.Seek(topLeftIndex, SeekOrigin.Begin);
            _stream.Read(result);
            _stream.Seek(tempCurrentIndex, SeekOrigin.Begin);
        }
        return result[0];
    }
}
