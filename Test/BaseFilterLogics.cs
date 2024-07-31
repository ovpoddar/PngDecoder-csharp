using PngDecoder.Models.Filters;

namespace Test;
public class BaseFilterLogics
{
    private MemoryStream _stream;
    public BaseFilterLogics()
    {
        _stream = new MemoryStream();
        _stream.WriteByte(2); _stream.WriteByte(1); _stream.WriteByte(2);
        _stream.WriteByte(2); _stream.WriteByte(1); _stream.WriteByte(2);
    }

    [Fact]
    public void LeftCheckOnFirstLine()
    {
        // arrange
        var filter = new BaseFilter(_stream);
        _stream.Seek(1, SeekOrigin.Begin);
        _ = _stream.ReadByte();
        // act
        var result = filter.GetLeftByte(3, 1);
        // assert
        Assert.Equal(0, result);
    }

    [Fact]
    public void LeftCheckOnSecondLine()
    {
        var filter = new BaseFilter(_stream);
        _stream.Seek(5, SeekOrigin.Begin);
        _ = _stream.ReadByte();
        // act
        var result = filter.GetLeftByte(3, 1);
        // assert
        Assert.Equal(1, result);
    }

    [Fact]
    public void LeftCheckOnFirstLineLittleInSide()
    {
        // arrange
        var filter = new BaseFilter(_stream);
        _stream.Seek(2, SeekOrigin.Begin);
        _ = _stream.ReadByte();
        // act
        var result = filter.GetLeftByte(3, 1);
        // assert
        Assert.Equal(1, result);
    }

    [Fact]
    public void LeftCheckOnSecondLineLittleInSide()
    {
        var filter = new BaseFilter(_stream);
        _stream.Seek(5, SeekOrigin.Begin);
        _ = _stream.ReadByte();
        // act
        var result = filter.GetLeftByte(3, 1);
        // assert
        Assert.Equal(1, result);
    }

    [Fact]
    public void TopCheckOnFirstLine()
    {
        // arrange
        var filter = new BaseFilter(_stream);
        _stream.Seek(1, SeekOrigin.Begin);
        _ = _stream.ReadByte();
        // act
        var result = filter.GetTopByte(3);
        // assert
        Assert.Equal(0, result);
    }

    [Fact]
    public void TopCheckOnFirstLineLittleInSide()
    {
        // arrange
        var filter = new BaseFilter(_stream);
        _stream.Seek(2, SeekOrigin.Begin);
        _ = _stream.ReadByte();
        // act
        var result = filter.GetTopByte(3);
        // assert
        Assert.Equal(0, result);
    }


    [Fact]
    public void TopCheckOnSecondLine()
    {
        var filter = new BaseFilter(_stream);
        _stream.Seek(4, SeekOrigin.Begin);
        // act
        var result = filter.GetTopByte(3);
        // assert
        Assert.Equal(2, result);
    }

    [Fact]
    public void TopCheckOnSecondLineLittleInSide()
    {
        var filter = new BaseFilter(_stream);
        _stream.Seek(5, SeekOrigin.Begin);
        _ = _stream.ReadByte();
        // act
        var result = filter.GetTopByte(3);
        // assert
        Assert.Equal(2, result);
    }

    [Fact]
    public void TopLeftCheckOnSecondLine()
    {
        // arrange
        var filter = new BaseFilter(_stream);
        _stream.Seek(5, SeekOrigin.Begin);
        // act
        var response = filter.GetTopLeftByte(3, 1);

        // assert

        Assert.Equal(0, response);
    }

    [Fact]
    public void TopLeftCheckSecondLineLittleInSide()
    {
        // arrange
        var filter = new BaseFilter(_stream);
        _stream.Seek(6, SeekOrigin.Begin);
        // act
        var response = filter.GetTopLeftByte(3, 1);
        // assert
        Assert.Equal(1, response);
    }

    [Fact]
    public void TopLeftCheckFirstLine()
    {
        // arrange
        var filter = new BaseFilter(_stream);
        _stream.Seek(2, SeekOrigin.Begin);
        // act
        var response = filter.GetTopLeftByte(3, 1);
        // assert
        Assert.Equal(0, response);
    }



    [Fact]
    public void TopLeftCheckFirstLineLittleInSide()
    {
        // arrange
        var filter = new BaseFilter(_stream);
        _stream.Seek(3, SeekOrigin.Begin);
        // act
        var response = filter.GetTopLeftByte(3, 1);
        // assert
        Assert.Equal(0, response);
    }

    [Fact]
    public void AfterLeftCheckPositionCheck()
    {
        // arrange
        var filter = new BaseFilter(_stream);
        _stream.Seek(0, SeekOrigin.Begin);

        // act
        var pos1 = _stream.ReadByte();
        var pos2 = _stream.ReadByte();
        var leftPos1 = filter.GetLeftByte(3, 1);
        var pos3 = _stream.ReadByte();
        var leftPos2 = filter.GetLeftByte(3, 1);


        var pos4 = _stream.ReadByte();
        var pos5 = _stream.ReadByte();
        var leftPos3 = filter.GetLeftByte(3, 1);
        var pos6 = _stream.ReadByte();
        var leftPos4 = filter.GetLeftByte(3, 1);
        // assert

        var check = pos1 == 2
            && pos2 == 1
            && pos3 == 2
            && pos4 == 2
            && pos5 == 1
            && pos6 == 2
            && leftPos1 == 0
            && leftPos2 == pos2
            && leftPos3 == 0
            && leftPos4 == pos5;
        Assert.True(check);
    }

    [Fact]
    public void AfterTopCheckPositionCheck()
    {
        // arrange
        var filter = new BaseFilter(_stream);
        _stream.Seek(0, SeekOrigin.Begin);

        // act
        var pos1 = _stream.ReadByte();
        var pos2 = _stream.ReadByte();
        var topPos1 = filter.GetTopByte(3);
        var pos3 = _stream.ReadByte();
        var topPos2 = filter.GetTopByte(3);

        var pos4 = _stream.ReadByte();
        var pos5 = _stream.ReadByte();
        var topPos3 = filter.GetTopByte(3);
        var pos6 = _stream.ReadByte();
        var topPos4 = filter.GetTopByte(3);

        // assert

        var check = pos1 == 2
            && pos2 == 1
            && pos3 == 2
            && pos4 == 2
            && pos5 == 1
            && pos6 == 2
            && topPos1 == 0
            && topPos2 == 0
            && topPos3 == 1
            && topPos4 == 2;
        Assert.True(check);
    }

    [Fact]
    public void AfterTopLeftCheckPositionCheck()
    {
        // arrange
        var filter = new BaseFilter(_stream);
        _stream.Seek(0, SeekOrigin.Begin);

        // act
        var pos1 = _stream.ReadByte();
        var pos2 = _stream.ReadByte();
        var topLeftPos1 = filter.GetTopLeftByte(3, 1);
        var pos3 = _stream.ReadByte();
        var topLeftPos2 = filter.GetTopLeftByte(3, 1);

        var pos4 = _stream.ReadByte();
        var pos5 = _stream.ReadByte();
        var topLeftPos3 = filter.GetLeftByte(3, 1);
        var pos6 = _stream.ReadByte();
        var topLeftPos4 = filter.GetLeftByte(3, 1);

        // assert
        var check = pos1 == 2
            && pos2 == 1
            && pos3 == 2
            && pos4 == 2
            && pos5 == 1
            && pos6 == 2
            && topLeftPos1 == 0
            && topLeftPos2 == 0
            && topLeftPos3 == 0
            && topLeftPos4 == 1;
        Assert.True(check);
    }

    [Fact]
    public void CheckWrite()
    {
        // arrange
        var filter = new BaseFilter(_stream);
        _stream.Seek(0, SeekOrigin.Begin);
        var response = _stream.ReadByte();
        // act
        filter.UnApply(10, 3);
        var response2 = _stream.ReadByte();
        _stream.Seek(0, SeekOrigin.Begin);
        var newResponse = _stream.ReadByte();
        // assert
        Assert.Equal(2, response);
        Assert.Equal(10, newResponse);
        Assert.Equal(1, response2);
        Assert.NotEqual(response, newResponse);
        Assert.NotEqual(response, response2);
    }
}
