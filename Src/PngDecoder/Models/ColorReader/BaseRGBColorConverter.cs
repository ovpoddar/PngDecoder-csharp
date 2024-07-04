using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PngDecoder.Models.ColorReader;
public abstract class BaseRGBColorConverter
{
    public abstract void Write(byte[] result, byte inputByte, ref int index);
    public (byte? step, byte? mask) BitDepthDetails(byte bitDepth)
    {
        if (bitDepth < 8)
        {
            byte step = 1;
            for (byte i = 0; i < bitDepth; i++)
                step |= (byte)(1 << i);

            return ((byte)(8 - bitDepth), step);
        }
        return (null, null);
    }
}
