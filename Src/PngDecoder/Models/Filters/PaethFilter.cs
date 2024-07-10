// Ignore Spelling: Paeth

namespace PngDecoder.Models.Filters;
internal class PaethFilter : BaseFilter
{
    private readonly byte _pixelSize;

    public PaethFilter(Stream stream, byte pixelSize) : base(stream) =>
        _pixelSize = pixelSize;

    public override byte UnApply(byte current, int scanLineWidth)
    {
        current = (byte)(current + PaethCalculate(
            GetLeftByte(scanLineWidth, _pixelSize),
            GetTopByte(scanLineWidth),
            GetTopLeftByte(scanLineWidth, _pixelSize)));
        return base.UnApply(current, scanLineWidth);
    }

    private static byte PaethCalculate(byte left, byte top, byte upperLeft)
    {
        var p = left + top - upperLeft;
        var pa = Math.Abs(p - left);
        var pb = Math.Abs(p - top);
        var pc = Math.Abs(p - upperLeft);
        if (pa <= pb && pa <= pc)
            return left;
        else if (pb <= pc)
            return top;
        else
            return upperLeft;
    }
}
