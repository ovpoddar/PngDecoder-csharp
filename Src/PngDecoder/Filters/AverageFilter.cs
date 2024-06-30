using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PngDecoder.Filters;
internal class AverageFilter : BaseFilter
{
    public AverageFilter(Stream stream) : base(stream) { }

    public override void Apply(byte current, int scanLineWidth)
    {
        current = (byte)(current
            + (GetLeftByte(scanLineWidth) + GetTopByte(scanLineWidth))
            / 2);
        base.Apply(current, scanLineWidth);
    }
}
