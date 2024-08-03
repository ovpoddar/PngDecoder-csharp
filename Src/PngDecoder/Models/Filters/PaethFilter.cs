

namespace PngDecoder.Models.Filters;

internal class PaethFilter : BasePNGFilter
{
    private byte[]? _leftPixel;
    public PaethFilter(Stream stream) : base(stream)
    {
        _leftPixel = null;
    }

    internal override Span<byte> UnApply(Span<byte> currentPixel, int scanlineWidth)
    {
        var pixelLength = currentPixel.Length;

        if (_leftPixel == null || _leftPixel.Length != pixelLength)
            _leftPixel = new byte[pixelLength];

        Span<byte> topLeftPixel = stackalloc byte[pixelLength];
        base.GetTopLeftPixel(topLeftPixel, scanlineWidth);

        Span<byte> topPixel = stackalloc byte[pixelLength];
        base.GetTopPixel(topPixel, scanlineWidth);

        for (byte i = 0; i < currentPixel.Length; i++)
            currentPixel[i] = (byte)(PaethCalculate(_leftPixel[i], topPixel[i], topLeftPixel[i]) + currentPixel[i]);

        currentPixel.CopyTo(_leftPixel);
        return base.UnApply(currentPixel, scanlineWidth);
    }

    static byte PaethCalculate(byte left, byte top, byte topLeft)
    {
        var p = left + top - topLeft;
        var pa = Math.Abs(p - left);
        var pb = Math.Abs(p - top);
        var pc = Math.Abs(p - topLeft);
        if (pa <= pb && pa <= pc)
            return left;
        else if (pb <= pc)
            return top;
        else
            return topLeft;
    }
}