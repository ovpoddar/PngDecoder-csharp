namespace PngDecoder.Models;

/// <summary>
/// All the data blocks which exist a PNG file
/// </summary>
internal enum PngChunkType : int
{
    IHDR = 1380206665,
    cHRM = 1297238115,
    gAMA = 1095582055,
    sRGB = 1111970419,
    sBIT = 1414087283,
    PLTE = 1163152464,
    bKGD = 1145523042,
    hIST = 1414744424,
    tRNS = 1397641844,
    oFFs = 1933985391,
    pHYs = 1935231088,
    sCAL = 1279345523,
    IDAT = 1413563465,
    tIME = 1162692980,
    tEXt = 1951942004,
    zTXt = 1951945850,
    fRAc = 1665225318,
    gIFg = 1732659559,
    gIFt = 1950763367,
    gIFx = 2017872231,
    IEND = 1145980233,
    iTXt = 1951945833
}
