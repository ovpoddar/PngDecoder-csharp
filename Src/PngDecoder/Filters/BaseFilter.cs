namespace PngDecoder.Filters;
public class BaseFilter
{
    public virtual void Apply(Stream stream, byte current, int scanLineWidth)
    {
        // write can be here
    }

    public byte GetLeftByte(Stream stream, int lineWidth)
    {
        Span<byte> result = stackalloc byte[1];
        if (stream.Position % lineWidth != 2)
        {
            stream.Seek(-2, SeekOrigin.Current);
            stream.Read(result);
            stream.Seek(1, SeekOrigin.Current);
        }
        return result[0];
    }

    public byte GetTopByte(Stream stream, int lineWidth)
    {
        var topIndex = (stream.Position - lineWidth - 1);
        Span<byte> result = stackalloc byte[1];
        if (topIndex >= 0)
        {
            var tempCurrentIndex = stream.Position;
            stream.Seek(topIndex, SeekOrigin.Begin);
            stream.Read(result);

            stream.Seek(tempCurrentIndex, SeekOrigin.Begin);
        }
        return result[0];
    }

    public byte GetTopLeftByte(Stream stream, int lineWidth)
    {
        var topLeftIndex = (stream.Position - lineWidth - 2);
        Span<byte> result = stackalloc byte[1];
        // CAN BE SIMPLIFY WITHOUT CALCULATION
        if (
            //topLeftIndex >= 0 || // replacement
            stream.Position / lineWidth != 1 ||
            stream.Position % lineWidth != 2)
        {
            var tempCurrentIndex = stream.Position;

            stream.Seek(topLeftIndex, SeekOrigin.Begin);
            stream.Read(result);
            stream.Seek(tempCurrentIndex, SeekOrigin.Begin);
        }
        return result[0];
    }
}
