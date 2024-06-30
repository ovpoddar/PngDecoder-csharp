using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PngDecoder.Filters;
internal class SubFilter : BaseFilter
{
    public override void Apply(Stream stream, byte current, int scanLineWidth)
    {
        var c = GetLeftByte(stream, scanLineWidth);
        base.Apply(stream, current, scanLineWidth);
    }
}
