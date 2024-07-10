namespace PngDecoder.Models.Filters;
internal class SubFilter : BaseFilter
{
    private readonly byte _pixelSize;

    public SubFilter(Stream stream, byte pixelSize) : base(stream) =>
        _pixelSize = pixelSize;

    public override byte UnApply(byte current, int scanLineWidth)
    {
        current = (byte)(GetLeftByte(scanLineWidth, _pixelSize) + current);
        return base.UnApply(current, scanLineWidth);
    }
}
