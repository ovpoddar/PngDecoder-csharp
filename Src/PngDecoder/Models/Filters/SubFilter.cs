

namespace PngDecoder.Models.Filters;

internal class SubFilter : BasePNGFilter
{
    private byte[]? _leftPixel;
    public SubFilter(Stream stream) : base(stream) 
    { 
        _leftPixel = null; 
    }

    internal override Span<byte> UnApply(Span<byte> currentPixel, int scanlineWidth)
    {
        if (_leftPixel == null || _leftPixel.Length != currentPixel.Length)
            _leftPixel = new byte[currentPixel.Length];

        for (var i = 0; i < currentPixel.Length; i++)
            currentPixel[i] = (byte)(_leftPixel[i] + currentPixel[i]);

        currentPixel.CopyTo(_leftPixel);
        return base.UnApply(currentPixel, scanlineWidth);
    } 
}