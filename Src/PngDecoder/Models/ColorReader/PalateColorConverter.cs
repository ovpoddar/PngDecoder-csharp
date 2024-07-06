using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace PngDecoder.Models.ColorReader;
internal class PalateColorConverter : BaseRGBColorConverter
{
    private readonly PLTEData _data;

    public PalateColorConverter(PLTEData data, IHDRData ihdr) : base(ihdr) =>
        _data = data;

    public override void Write(Span<byte> result, byte inputByte, ref int writeIndex)
    {
        var bitDetails = base.BitDepthDetails();
        if (bitDetails is { mask: not null, step: not null })
        {
            // less than 8 n
            for (int j = bitDetails.step!.Value; j >= 0; j -= Ihdr.BitDepth)
            {
                byte mask = (byte)(bitDetails.mask << j);
                byte currentBit = (byte)((inputByte & mask) >> j);
                var colors = _data[currentBit];

                if (writeIndex < Ihdr.Width * 4)
                {
                    for (int i = 0; i < colors.Length; i++)
                    {
                        result[writeIndex] = colors[i];
                        writeIndex++;
                    }
                    // for alpha
                    result[writeIndex++] = 255;
                }
            }
        }
        else if (Ihdr.BitDepth == 8)
        {
            if (writeIndex % 4 == 3)
            {
                result[writeIndex] = 255;
                writeIndex++;
            }
            result[writeIndex] = inputByte;
            writeIndex++;
        }
    }

}
