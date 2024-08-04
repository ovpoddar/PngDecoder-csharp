using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PngDecoder.Models.ColorReader;
internal class RGBColorConverter : BaseRGBColorConverter
{

    public RGBColorConverter(IHDRData headerData) : base(headerData) { }

    internal override void Write(Span<byte> result, Span<byte> currentByte, ref int writingIndex)
    {
        if (base.HeaderData.BitDepth == 8)
        {
            Debug.Assert(currentByte.Length == 3);

            result[writingIndex++] = currentByte[0];
            result[writingIndex++] = currentByte[1];
            result[writingIndex++] = currentByte[2];
            result[writingIndex++] = 255;
        }
        // 16
        else
        {
            Debug.Assert(currentByte.Length == 6);

            result[writingIndex++] = currentByte[0];
            result[writingIndex++] = currentByte[2];
            result[writingIndex++] = currentByte[4];
            result[writingIndex++] = 255;
        }
    }
}
