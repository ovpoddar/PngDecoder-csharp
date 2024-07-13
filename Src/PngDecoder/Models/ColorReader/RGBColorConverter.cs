// Ignore Spelling: ihdr

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PngDecoder.Models.ColorReader;
internal class RGBColorConverter : BaseRGBColorConverter
{
    public RGBColorConverter(IHDRData ihdr) : base(ihdr) { }

    public override void Write(Span<byte> result, byte inputByte, ref int writeIndex)
    {
        if (base.Ihdr.BitDepth == 8)
        {
            if (writeIndex % 4 == 3)
            {
                result[writeIndex] = 255;
                writeIndex++;
            }
            result[writeIndex] = inputByte;
            writeIndex++;
        }
        // 16
        else
        {
            if (_canRead)
            {
                result[writeIndex++] = inputByte;
                if (writeIndex % 4 == 3)
                {
                    result[writeIndex++] = 255;
                }
            }
            _canRead = !_canRead;
        }
    }
    private bool _canRead = true;
}
