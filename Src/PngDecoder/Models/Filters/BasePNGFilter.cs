using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PngDecoder.Models.Filters;
public class BasePNGFilter
{
    private readonly Stream _stream;

    public BasePNGFilter(Stream stream) =>
        _stream = stream;

    internal virtual Span<byte> UnApply(Span<byte> currentPixel, int scanlineWidth)
    {
        _stream.Seek(-currentPixel.Length, SeekOrigin.Current);
        _stream.Write(currentPixel);
        return currentPixel;
    }

    public void GetLeftPixel(Span<byte> result, int scanLineWidth)
    {
        var modPosation = _stream.Position % scanLineWidth;
        var pixelLength = result.Length;
        if (modPosation <= pixelLength + 1 && modPosation > 0)
            return;

        var tempPosation = _stream.Position;
        _stream.Seek(-pixelLength, SeekOrigin.Current);
        _stream.Read(result);
        _stream.Seek(tempPosation, SeekOrigin.Begin);
    }

    public void GetTopPixel(Span<byte> result, int scanLineWidth)
    {
        // (scanLineWidth + 1) for next account next filter
        if (_stream.Position <= (scanLineWidth + 1))
            return;

        var tempPosation = _stream.Position;
        _stream.Seek(-(result.Length + scanLineWidth), SeekOrigin.Current);
        _stream.Read(result);
        _stream.Seek(tempPosation, SeekOrigin.Begin);
    }
   
    public void GetTopLeftPixel(Span<byte> result, int scanLineWidth)
    {
        var currentPosition = _stream.Position;
        var pixelLength = result.Length;
        if (currentPosition <= scanLineWidth + 1 + pixelLength)
            return;

        _stream.Seek(currentPosition - (2 * pixelLength + scanLineWidth), SeekOrigin.Begin);
        _stream.Read(result);
        _stream.Position = currentPosition;
    }
}
