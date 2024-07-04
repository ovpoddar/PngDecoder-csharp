using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PngDecoder.Models.ColorReader;
internal class PalateColorConverter : BaseRGBColorConverter
{
    private readonly PLTEData _data;
    private readonly IHDRData _ihdr;

    public PalateColorConverter(PLTEData data, IHDRData ihdr)
    {
        _data = data;
        _ihdr = ihdr;
    }

    public override void Write(byte[] result, byte inputByte, ref int index)
    {
        var bitDetails = base.BitDepthDetails(_ihdr.BitDepth);
        if (bitDetails is { mask: not null, step: not null })
        {
            // less than 8 n
            for (int j = bitDetails.step!.Value; j >= 0; j -= _ihdr.BitDepth)
            {
                byte mask = (byte)(bitDetails.mask << j);
                byte currentBit = (byte)((inputByte & mask) >> j);
                var colorIndex = currentBit * 3;
                var colors = _data[colorIndex];
                for (int i = 0; i < colors.Length; i++)
                {
                    /* implement this logic too
                     * var color = GetTrueColor(palate, newPixel);
                    if (totalWrite < width)
                    {
                        output[index] = color;
                        current[i] = newPixel;
                        index++;
                    }*/
                    // todo add alfa logic too
                    result[index] = colors[i];
                    index++;
                }
            }
        }
        else if (_ihdr.BitDepth == 8)
        {
            // todo add alfa logic too
            result[index] = inputByte;
            index++;
        }
    }

}
