using PngDecoder.Models.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Test;
public class BaseFilterForSixteenBitImage
{

    private MemoryStream _stream;
    public BaseFilterForSixteenBitImage()
    {
        _stream = new MemoryStream();
        _stream.Position = 0;

        _stream.WriteByte(99); // Filter              
            _stream.WriteByte(8); // R
            _stream.WriteByte(9); // G
            _stream.WriteByte(10); // B

            _stream.WriteByte(8); // R
            _stream.WriteByte(9); // G
            _stream.WriteByte(10); // B

            _stream.WriteByte(8); // R
            _stream.WriteByte(9); // G
            _stream.WriteByte(10); // B

        _stream.WriteByte(99); // Filter    
            _stream.WriteByte(18); // R
            _stream.WriteByte(19); // G
            _stream.WriteByte(20); // B

            _stream.WriteByte(18); // R
            _stream.WriteByte(19); // G
            _stream.WriteByte(20); // B

            _stream.WriteByte(18); // R
            _stream.WriteByte(19); // G
            _stream.WriteByte(20); // B
    }

    [Fact]
    public void Get3PartLeft()
    {
        var filter = new BaseFilter(_stream);
        _stream.Seek(4, SeekOrigin.Begin);
        var pos  = _stream.ReadByte();
        var left = filter.GetLeftByte(9, 3);

        Assert.Equal(pos, left);
    }
}
