

namespace PngDecoder.Models.Filters;

internal class AverageFilter : BasePNGFilter
{
    public AverageFilter(Stream stream) : base(stream)
    {
    }

    internal override Span<byte> UnApply(Span<byte> currentPixel, int scanlineWidth)
    {
        var pixelLength = currentPixel.Length;

        Span<byte> leftPixel = stackalloc byte[pixelLength];
        base.GetLeftPixel(leftPixel, scanlineWidth);

        Span<byte> topPixel = stackalloc byte[pixelLength];
        base.GetTopPixel(topPixel, scanlineWidth);

        for (byte i = 0; i < currentPixel.Length; i++)
            currentPixel[i] = (byte)((leftPixel[i] + currentPixel[i] + topPixel[i]) /2);

        return base.UnApply(currentPixel, scanlineWidth);
    }
}