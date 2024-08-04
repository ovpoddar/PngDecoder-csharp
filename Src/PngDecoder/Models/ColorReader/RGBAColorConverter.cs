using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PngDecoder.Models.ColorReader;
internal class RGBAColorConverter : BaseRGBColorConverter
{
    public RGBAColorConverter(IHDRData headerData) : base(headerData) { }

    internal override void Write(Span<byte> result, Span<byte> currentByte, ref int writingIndex)
    {
        if (base.HeaderData.BitDepth == 8)
        {
            Debug.Assert(currentByte.Length == 4);
            result[writingIndex++] = currentByte[0];
            result[writingIndex++] = currentByte[1];
            result[writingIndex++] = currentByte[2];
            result[writingIndex++] = currentByte[3];
        }
        else // 16
        {
            Debug.Assert(currentByte.Length == 8);
            result[writingIndex++] = currentByte[0];
            result[writingIndex++] = currentByte[2];
            result[writingIndex++] = currentByte[4];
            result[writingIndex++] = currentByte[6];
        }
    }
}
