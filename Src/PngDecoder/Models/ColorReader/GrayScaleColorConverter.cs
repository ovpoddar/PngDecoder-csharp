using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PngDecoder.Models.ColorReader;
internal class GrayScaleColorConverter : BaseRGBColorConverter
{
    public GrayScaleColorConverter(IHDRData headerData) : base(headerData) { }

    internal override void Write(Span<byte> result, Span<byte> currentByte, ref int writingIndex)
    {
        var bitDetails = base.BitDepthDetailsForGrayScale();
        if (bitDetails is { mask: not null, bit: not null, map: not null })
        {
            Debug.Assert(currentByte.Length == 1);
            for (int j = 0; j < 8; j += bitDetails.bit!.Value)
            {
                byte currentBit = (byte)((byte)(((currentByte[0]) >> (8 - bitDetails.bit - j)) & bitDetails.mask) * (255 / bitDetails.map));
                if (writingIndex < base.HeaderData.Width * 4)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        result[writingIndex++] = currentBit;
                    }
                    // for alpha
                    result[writingIndex++] = 255;
                }
            }
        }
        else if (HeaderData.BitDepth == 8)
        {
            Debug.Assert(currentByte.Length == 1);
            for (int i = 0; i < 3; i++)
            {
                result[writingIndex++] = currentByte[0];
            }

            result[writingIndex++] = 255;
        }
        else // 16
        {
            Debug.Assert(currentByte.Length == 2);
            for (int i = 0; i < 3; i++)
            {
                result[writingIndex++] = currentByte[0];
            }
            result[writingIndex++] = 255;

        }
    }
}
