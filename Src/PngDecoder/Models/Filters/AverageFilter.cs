namespace PngDecoder.Models.Filters;
internal class AverageFilter : BaseFilter
{
    public AverageFilter(Stream stream) : base(stream) { }

    public override byte UnApply(byte current, int scanLineWidth)
    {
        current = (byte)(current
            + (GetLeftByte(scanLineWidth) + GetTopByte(scanLineWidth))
            / 2);
        return base.UnApply(current, scanLineWidth);
    }
}
