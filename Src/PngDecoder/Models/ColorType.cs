using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PngDecoder.Models;
internal enum ColorType : byte
{
    GreyScale = 0,                              // Gray Scale
    RGB = 2,                                    // True Color
    Palette = 3,                                // Palette	
    GreyScaleAndAlpha = 4,                      // Gray Scale and alpha
    RGBA = 6                                    // True Color and alpha
}
