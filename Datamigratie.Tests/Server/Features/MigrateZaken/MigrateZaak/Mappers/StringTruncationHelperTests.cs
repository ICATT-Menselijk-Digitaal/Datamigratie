using Datamigratie.Server.Features.MigrateZaken.MigrateZaak.Mappers;

namespace Datamigratie.Tests.Server.Features.MigrateZaken.MigrateZaak.Mappers;

public class StringTruncationHelperTests
{
    [Fact]
    public void TruncateWithDots_ShortString_ReturnsOriginal()
    {
        var result = StringTruncationHelper.TruncateWithDots("hello", 10);
        Assert.Equal("hello", result);
    }

    [Fact]
    public void TruncateWithDots_ExactLength_ReturnsOriginal()
    {
        var result = StringTruncationHelper.TruncateWithDots("hello", 5);
        Assert.Equal("hello", result);
    }

    [Fact]
    public void TruncateWithDots_LongString_TruncatesWithDots()
    {
        var result = StringTruncationHelper.TruncateWithDots("hello world", 5);
        Assert.Equal("he...", result);
        Assert.Equal(5, result!.Length);
    }

    [Fact]
    public void TruncateWithDots_Null_ReturnsNull()
    {
        var result = StringTruncationHelper.TruncateWithDots(null, 10);
        Assert.Null(result);
    }

    [Fact]
    public void TruncateWithDots_MaxLengthSmallerThanDots_ReturnsDots()
    {
        var result = StringTruncationHelper.TruncateWithDots("hello", 2);
        Assert.Equal("...", result);
    }
}
