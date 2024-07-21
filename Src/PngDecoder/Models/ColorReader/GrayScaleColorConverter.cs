namespace PngDecoder.Models.ColorReader;
internal class GrayScaleColorConverter : BaseRGBColorConverter
{
    public GrayScaleColorConverter(IHDRData ihdr) : base(ihdr) { }
    public override void Write(Span<byte> result, byte inputByte, ref int writeIndex)
    {
        var bitDetails = base.BitDepthDetailsForGrayScale();
        if (bitDetails is { mask: not null, bit: not null, map: not null })
        {
            for (int j = 0; j < 8; j += bitDetails.bit!.Value)
            {
                byte currentBit = (byte)((byte)(((inputByte) >> (8 - bitDetails.bit - j)) & bitDetails.mask) * (255 / bitDetails.map));
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
