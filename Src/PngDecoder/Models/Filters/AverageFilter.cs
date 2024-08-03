

namespace PngDecoder.Models.Filters;

internal class AverageFilter : BasePNGFilter
{
    private byte[]? _leftPixel;
    public AverageFilter(Stream stream) : base(stream)
    {
        _leftPixel = null;
    }

    internal override Span<byte> UnApply(Span<byte> currentPixel, int scanlineWidth)
    {
        var pixelLength = currentPixel.Length;

        if (_leftPixel == null || _leftPixel.Length != pixelLength)
            _leftPixel = new byte[pixelLength];

        Span<byte> topPixel = stackalloc byte[pixelLength];
        base.GetTopPixel(topPixel, scanlineWidth);

        for (byte i = 0; i < currentPixel.Length; i++)
            currentPixel[i] = (byte)((_leftPixel[i] + currentPixel[i] + topPixel[i]) /2);

        currentPixel.CopyTo(_leftPixel);
        return base.UnApply(currentPixel, scanlineWidth);
    }
}