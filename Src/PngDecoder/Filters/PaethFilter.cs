// Ignore Spelling: Paeth

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PngDecoder.Filters;
internal class PaethFilter : BaseFilter
{
    public PaethFilter(Stream stream) : base(stream) { }

    public override void Apply(byte current, int scanLineWidth)
    {
        current = (byte)(current + PaethCalculate(
            GetLeftByte(scanLineWidth),
            GetTopByte(scanLineWidth),
            GetTopLeftByte(scanLineWidth)));
        base.Apply(current, scanLineWidth);
    }

    private static byte PaethCalculate(byte left, byte top, byte upperLeft)
    {
        var p = left + top - upperLeft;
        var pa = Math.Abs(p - left);
        var pb = Math.Abs(p - top);
        var pc = Math.Abs(p - upperLeft);
        if (pa <= pb && pa <= pc)
            return left;
        else if (pb <= pc)
            return top;
        else
            return upperLeft;
    }
}
