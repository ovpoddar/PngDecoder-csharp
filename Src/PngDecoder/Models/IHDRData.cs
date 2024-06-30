﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PngDecoder.Models;
internal struct IHDRData
{
    public uint Width { get; set; }
    public uint Height { get; set; }
    public byte BitDepth { get; set; }
    public ColorType ColorType { get; set; }
    public byte CompressionMethod { get; set; }
    public byte FilterMethod { get; set; }
    public byte InterlaceMethod { get; set; }

    public IHDRData(PNGChunk headerChunk)
    {
        if (headerChunk.Length != 13)
            throw new ArgumentException("Invalid Header");

        Span<byte> response = stackalloc byte[(int)headerChunk.Length];
        headerChunk.GetData(response);

        var temp = response.Slice(0, 4);
        temp.Reverse();
        this.Width = BitConverter.ToUInt32(temp);

        temp = response.Slice(4, 4);
        temp.Reverse();
        this.Height = BitConverter.ToUInt32(temp);

        this.BitDepth = response[8];
        this.ColorType = (ColorType)response[9];
        this.CompressionMethod = response[10];
        this.FilterMethod = response[11];
        this.InterlaceMethod = response[12];

    }

    public int GetScanLinesWidth()
    {
        var length = Width * BitDepth * GetBytePerPixels;
        var count = (int)(length / 8);
        var extra = length % 8;

        if (extra == 0)
            return count;
        return ++count;
    }

    private readonly uint GetBytePerPixels => ColorType switch
    {
        ColorType.GreyScale => 1,
        ColorType.RGB => 3,
        ColorType.Palette => 1,
        ColorType.GreyScaleAndAlpha => 2,
        ColorType.RGBA => 4,
        _ => throw new Exception(),
    };

}