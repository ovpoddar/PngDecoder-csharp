﻿// Ignore Spelling: Ihdr

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PngDecoder.Models.ColorReader;
internal abstract class BaseRGBColorConverter
{
    public readonly IHDRData Ihdr;

    protected BaseRGBColorConverter(IHDRData ihdr) =>
        Ihdr = ihdr;

    public abstract void Write(Span<byte> result, byte inputByte, ref int writeIndex);
    public (byte? step, byte? mask) BitDepthDetailsForPalated()
    {
        if (Ihdr.BitDepth < 8)
        {
            byte step = 1;
            for (byte i = 0; i < Ihdr.BitDepth; i++)
                step |= (byte)(1 << i);

            return ((byte)(8 - Ihdr.BitDepth), step);
        }
        return (null, null);
    }

    public (byte? mask, byte? bit, byte? map) BitDepthDetailsForGrayScale()
    {
        if (Ihdr.BitDepth < 8)
        {
            return ((byte)(0xFF >> (8 - Ihdr.BitDepth)), Ihdr.BitDepth, (byte)((1 << Ihdr.BitDepth) - 1));
        }
        return (null, null, null);
    }
}
