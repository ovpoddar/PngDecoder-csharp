namespace PngDecoder.Models.Filters;
internal class AverageFilter : BaseFilter
{
    private readonly byte _pixelSize;

    public AverageFilter(Stream stream, byte pixelSize) : base(stream) =>
        _pixelSize = pixelSize;

    public override byte UnApply(byte current, int scanLineWidth)
    {
        current = (byte)(current
            + (GetLeftByte(scanLineWidth, _pixelSize) + GetTopByte(scanLineWidth))
            / 2);
        return base.UnApply(current, scanLineWidth);
    }
}
