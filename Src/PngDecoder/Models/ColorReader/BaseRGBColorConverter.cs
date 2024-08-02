using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PngDecoder.Models.ColorReader;
internal abstract class BaseRGBColorConverter
{
    public readonly IHDRData HeaderData;

    protected BaseRGBColorConverter(IHDRData headerData) => 
        HeaderData = headerData;

    internal abstract void Write(Span<byte> result, Span<byte> currentByte, ref int writingIndex);

    public (byte? step, byte? mask) BitDepthDetailsForPalated()
    {
        if (HeaderData.BitDepth < 8)
        {
            byte step = 1;
            for (byte i = 0; i < HeaderData.BitDepth; i++)
                step |= (byte)(1 << i);

            return ((byte)(8 - HeaderData.BitDepth), step);
        }
        return (null, null);
    }

    public (byte? mask, byte? bit, byte? map) BitDepthDetailsForGrayScale()
    {
        if (HeaderData.BitDepth < 8)
        {
            return ((byte)(0xFF >> (8 - HeaderData.BitDepth)), HeaderData.BitDepth, (byte)((1 << HeaderData.BitDepth) - 1));
        }
        return (null, null, null);
    }
}
