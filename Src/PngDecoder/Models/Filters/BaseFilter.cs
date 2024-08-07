﻿namespace PngDecoder.Models.Filters;
public class BaseFilter
{
    private readonly Stream _stream;
    private const byte FILTERTYPEFACTOR = 1; 

    public BaseFilter(Stream stream) =>
        _stream = stream;

    public virtual byte UnApply(byte current, int scanLineWidth)
    {
        _stream.Seek(-1, SeekOrigin.Current);
        _stream.WriteByte(current);
        return current;
    }

    public byte GetLeftByte(int lineWidth, byte pixelLength)
    {
        // need account the bit depth
        Span<byte> result = stackalloc byte[1];
        var modPosation = _stream.Position % lineWidth;
        if (modPosation > (pixelLength + FILTERTYPEFACTOR) || modPosation == 0)
        {
            var tempPos = _stream.Position;
            _stream.Seek(-(pixelLength + FILTERTYPEFACTOR), SeekOrigin.Current);
            _stream.Read(result);
            _stream.Seek(tempPos, SeekOrigin.Begin);
        }
        return result[0];

    }

    public byte GetTopByte(int lineWidth)
    {
        var topIndex = _stream.Position - lineWidth - 1;
        Span<byte> result = stackalloc byte[1];
        if (topIndex >= 0)
        {
            var tempCurrentIndex = _stream.Position;
            _stream.Seek(topIndex, SeekOrigin.Begin);
            _stream.Read(result);

            _stream.Seek(tempCurrentIndex, SeekOrigin.Begin);
        }
        return result[0];
    }

    public byte GetTopLeftByte(int lineWidth, byte pixelLength)
    {
        Span<byte> result = stackalloc byte[1];
        var topleftIndex = lineWidth + FILTERTYPEFACTOR + pixelLength;
        var modPosation = (_stream.Position - lineWidth) % lineWidth;
        if (_stream.Position > lineWidth && (modPosation > (pixelLength + FILTERTYPEFACTOR) || modPosation == 0))
        {

            var tempPos = _stream.Position;
            _stream.Seek(-(topleftIndex), SeekOrigin.Current);
            _stream.Read(result);
            _stream.Seek(tempPos, SeekOrigin.Begin);
        }
        return result[0];
    }
}
