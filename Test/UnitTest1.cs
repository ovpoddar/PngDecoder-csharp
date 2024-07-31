using PngDecoder;
using System.Diagnostics;

namespace Test;

public class UnitTest1
{
    private IEnumerable<string> _list;
    public UnitTest1()
    {
        var folder = "D:\\testP\\Png";
        _list = Directory.GetFiles(folder);
    }

    [Fact]
    public void TotalFiles()
    {
        Assert.Equal(262, _list.Count());
    }

    [Fact]
    public void FirstBatch()
    {
        // arrange
        // act
        foreach (var file in _list)
        {
            using (var fs = File.Open(file, FileMode.Open, FileAccess.Read))
            {
                try
                {
                    var pngDecode = new PNGDecode(fs);
                    var c = pngDecode.DecodeImageData();

                    // assert
                    Assert.NotEmpty(c);
                }
                catch (Exception ex)
                {
                    var msg = $"{file} {ex.Message}";
                    Console.WriteLine(msg);
                }
                fs.Close();

            }
        }
        Debug.Assert(0 != _list.Count());
    }


}