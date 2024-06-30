using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PngDecoder.Models;
internal readonly struct PLTEData
{
    public readonly byte[] Palette;
    public PLTEData(PNGChunk palate)
    {
        Palette = new byte[palate.Length];
        palate.GetData(Palette);
    }

    // r,g,b format
    public readonly ReadOnlySpan<byte> this[int index] =>
        new ReadOnlySpan<byte>(Palette, index *= 3, 3);
}
