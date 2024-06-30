namespace PngDecoder.Models.Filters;
internal class UpFilter : BaseFilter
{
    public UpFilter(Stream stream) : base(stream) { }

    public override byte UnApply(byte current, int scanLineWidth)
    {
        current = (byte)(GetTopByte(scanLineWidth) + current);
        return base.UnApply(current, scanLineWidth);
    }
}
