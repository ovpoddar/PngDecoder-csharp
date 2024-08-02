

namespace PngDecoder.Models.Filters;

internal class UpFilter : BasePNGFilter
{
    public UpFilter(Stream stream) : base(stream)
    {
    }

    internal override Span<byte> UnApply(Span<byte> currentPixel, int scanlineWidth)
    {
        var pixelLength = currentPixel.Length;
        Span<byte> topPixel = stackalloc byte[pixelLength];
        base.GetTopPixel(topPixel, scanlineWidth);
        for (byte i = 0; i < currentPixel.Length; i++)
            currentPixel[i] = (byte)(topPixel[i] + currentPixel[i]);

        return base.UnApply(currentPixel, scanlineWidth);
    }
}