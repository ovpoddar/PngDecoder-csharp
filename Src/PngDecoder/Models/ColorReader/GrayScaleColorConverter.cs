﻿namespace PngDecoder.Models.ColorReader;
internal class GrayScaleColorConverter : BaseRGBColorConverter
{
    public GrayScaleColorConverter(IHDRData ihdr) : base(ihdr) { }
    public override void Write(Span<byte> result, byte inputByte, ref int writeIndex)
    {
        var bitDetails = base.BitDepthDetails();
        if (bitDetails is { mask: not null, step: not null })
        {
            for (int j = bitDetails.step!.Value; j >= 0; j -= Ihdr.BitDepth)
            {
                byte mask = (byte)(bitDetails.mask << j);
                byte currentBit = (byte)((inputByte & mask) >> j);

                if (writeIndex < Ihdr.Width * 4)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        result[writeIndex] = currentBit;
                        writeIndex++;
                    }
                    // for alpha
                    result[writeIndex++] = 255;
                }
            }
        }
        else if (Ihdr.BitDepth == 8)
        {
            for (int i = 0; i < 3; i++)
            {
                result[writeIndex] = inputByte;
                writeIndex++;
            }

            result[writeIndex] = 255;
            writeIndex++;
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
                result[writeIndex] = 255;
                writeIndex++;
            }
            else
            {
                _cnt = 0;
                return;
            }
            _cnt++;
        }
    }

    private byte _cnt;
}
