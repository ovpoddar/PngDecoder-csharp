using ImageQT;
using System.Linq;
using ImageQT.Models.ImagqQT;
using PngDecoder;
using System.Text;
using System.Runtime.InteropServices;
using PngDecoder.Models;

var folder = "D:\\testP\\png\\broken";
var files = Directory.GetFiles(folder);
files = [@"D:\testP\png\broken\Issue_2589 (2).png"];
foreach (var file in files)
{
    try
    {
        using var fs = File.OpenRead(file);
        var p = new PNGDecode(fs);
        var rawdecode = p.DecodeImageData();
        var format = new Pixels[p.width * p.Height];
        for (int i = 0; i < format.Length; i++)
        {
            format[i] = new Pixels(rawdecode[i * 4], rawdecode[i * 4 + 1], rawdecode[i * 4 + 2]);
        }
        var img = ImageLoader.LoadImage(p.width, p.Height, ref format);
        var qt = new ImageQt(img);
        await qt.Show();
    }
    catch (Exception ex)
    {
        Console.BackgroundColor = ConsoleColor.Red;
        Console.WriteLine($"{file} {ex.Message }");
        Console.BackgroundColor = default(ConsoleColor);
    }
}
Console.ReadLine();