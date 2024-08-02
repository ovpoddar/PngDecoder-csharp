using System.Diagnostics;

namespace PngDecoder.Models.ColorReader;
internal class PalateColorConverter : BaseRGBColorConverter
{
    private readonly PLTEData _palate;

    public PalateColorConverter(PLTEData palate, IHDRData headerData) : base(headerData) =>
        _palate = palate;

    internal override void Write(Span<byte> result,
        Span<byte> currentByte, 
        ref int writingIndex)
    {
        var bitDetails = base.BitDepthDetailsForPalated();
        if (bitDetails is { mask: not null, step: not null })
        {
            Debug.Assert(currentByte.Length == 1);
            // less than 8 n
            for (int j = bitDetails.step!.Value; j >= 0; j -= HeaderData.BitDepth)
            {
                byte mask = (byte)(bitDetails.mask << j);
                byte currentBit = (byte)((currentByte[0] & mask) >> j);
                var colors = _palate[currentBit];

                if (writingIndex < HeaderData.Width * 4)
                {
                    for (int i = 0; i < colors.Length; i++)
                    {
                        result[writingIndex] = colors[i];
                        writingIndex++;
                    }
                    // for alpha
                    result[writingIndex++] = 255;
                }
            }
        }
        else if (HeaderData.BitDepth == 8)
        {
            Debug.Assert(currentByte.Length == 1);
            if (writingIndex % 4 == 3)
            {
                result[writingIndex] = 255;
                writingIndex++;
            }
            result[writingIndex] = _palate[currentByte[0]][0];
            writingIndex++;
            result[writingIndex] = _palate[currentByte[0]][1];
            writingIndex++;
            result[writingIndex] = _palate[currentByte[0]][2];
            writingIndex++;
        }
    }
}
