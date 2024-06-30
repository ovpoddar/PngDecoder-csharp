using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PngDecoder.Filters;
internal class UpFilter : BaseFilter
{
    public UpFilter(Stream stream) : base(stream) { }

    public override void Apply(byte current, int scanLineWidth)
    {
        current = (byte)(GetTopByte(scanLineWidth) + current);
        base.Apply(current, scanLineWidth);
    }
}
