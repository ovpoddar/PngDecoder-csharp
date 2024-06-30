namespace PngDecoder.Models.Filters;
internal class SubFilter : BaseFilter
{
    public SubFilter(Stream stream) : base(stream) { }

    public override byte UnApply(byte current, int scanLineWidth)
    {
        current = (byte)(GetLeftByte(scanLineWidth) + current);
        return base.UnApply(current, scanLineWidth);
    }
}
