

namespace PngDecoder.Models.Filters;

internal class SubFilter : BasePNGFilter
{
    public SubFilter(Stream stream) : base(stream)
    {
    }

    internal override Span<byte> UnApply(Span<byte> currentPixel, int scanlineWidth)
    {
        var pixelLength = currentPixel.Length;
        Span<byte> leftPixel = stackalloc byte[pixelLength];
        base.GetLeftPixel(leftPixel, scanlineWidth);
        for (byte i = 0; i < currentPixel.Length; i++)
            currentPixel[i] =(byte)(leftPixel[i] + currentPixel[i]);
        
        return base.UnApply(currentPixel, scanlineWidth);
    }
}