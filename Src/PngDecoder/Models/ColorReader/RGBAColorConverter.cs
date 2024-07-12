using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PngDecoder.Models.ColorReader;
internal class RGBAColorConverter : BaseRGBColorConverter
{
    public RGBAColorConverter(IHDRData ihdr) : base(ihdr) { }

    public override void Write(Span<byte> result, byte inputByte, ref int writeIndex)
    {
        if (base.Ihdr.BitDepth == 8)
        {
            result[writeIndex] = inputByte;
            writeIndex++;
        }
        else // 16
        {
            if (_cnt == 0)
            {
                result[writeIndex] = inputByte;
                writeIndex++;
                _cnt++;
            }
            else
            {
                _cnt = 0;
                return;
            }
        }
    }

    private byte _cnt;
}
