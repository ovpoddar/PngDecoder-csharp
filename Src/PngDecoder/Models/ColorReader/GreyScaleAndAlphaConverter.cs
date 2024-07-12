using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PngDecoder.Models.ColorReader;
internal class GreyScaleAndAlphaConverter : BaseRGBColorConverter
{
    public GreyScaleAndAlphaConverter(IHDRData ihdr) : base(ihdr) { }

    public override void Write(Span<byte> result, byte inputByte, ref int writeIndex)
    {
        if (base.Ihdr.BitDepth == 8)
        {
            if (writeIndex % 2 == 0)
            {
                for (int i = 0; i < 3; i++)
                {
                    result[writeIndex] = inputByte;
                    writeIndex++;
                }
            }
            else
            {
                result[writeIndex] = inputByte;
                writeIndex++;
            }
        }
        else // 16
        {
            if (_cnt == 0)
            {
                for (int i = 0; i < 3; i++)
                {
                    result[writeIndex] = inputByte;
                    writeIndex++;
                }
            }
            else if (_cnt == 2)
            {
                result[writeIndex] = inputByte;
                writeIndex++;
            }
            else if (_cnt == 3)
            {
                _cnt = 0;
                return;
            }
            _cnt++;
        }
    }

    private byte _cnt = 0;
}
