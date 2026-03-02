using Datamigratie.Server.Helpers;
using Microsoft.Extensions.Logging;
using Moq;

namespace Datamigratie.Tests.Server.Helpers;

public class RsinValidatorTests
{
    private readonly Mock<ILogger> _mockLogger;

    public RsinValidatorTests()
    {
        _mockLogger = new Mock<ILogger>();
    }

    [Theory]
    [InlineData("123456782")] // Valid RSIN that passes 11-test
    [InlineData("000000000")] // Edge case: all zeros (passes 11-test)
    public void ValidateRsin_ValidRsin_DoesNotThrow(string validRsin)
    {
        // Act & Assert - Should not throw
        var exception = Record.Exception(() => RsinValidator.ValidateRsin(validRsin, _mockLogger.Object));
        Assert.Null(exception);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void ValidateRsin_NullOrWhitespace_ThrowsArgumentException(string? rsin)
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => RsinValidator.ValidateRsin(rsin, _mockLogger.Object));
        Assert.Equal("RSIN mag niet leeg zijn.", exception.Message);
    }

    [Theory]
    [InlineData("12345678")]  // Too short
    [InlineData("1234567890")] // Too long
    [InlineData("123")]        // Way too short
    public void ValidateRsin_IncorrectLength_ThrowsArgumentException(string rsin)
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => RsinValidator.ValidateRsin(rsin, _mockLogger.Object));
        Assert.Equal("RSIN moet precies 9 cijfers bevatten.", exception.Message);
    }

    [Theory]
    [InlineData("12345678a")]
    [InlineData("ABCDEFGHI")]
    [InlineData("123 456 7")]
    [InlineData("123-456-7")]
    public void ValidateRsin_ContainsNonDigits_ThrowsArgumentException(string rsin)
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => RsinValidator.ValidateRsin(rsin, _mockLogger.Object));
        Assert.Equal("RSIN mag alleen cijfers bevatten.", exception.Message);
    }

    [Theory]
    [InlineData("123456789")] // Fails 11-test
    [InlineData("111111111")] // Fails 11-test
    [InlineData("987654321")] // Fails 11-test
    public void ValidateRsin_FailsElevenTest_ThrowsArgumentException(string rsin)
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => RsinValidator.ValidateRsin(rsin, _mockLogger.Object));
        Assert.Equal("RSIN is niet geldig volgens de 11-proef.", exception.Message);
    }

    [Fact]
    public void ValidateRsin_ValidRealWorldExample_DoesNotThrow()
    {
        // Arrange - Real RSIN example that passes 11-test
        // 9*1 + 8*2 + 7*3 + 6*4 + 5*5 + 4*6 + 3*7 + 2*8 + (-1)*2 = 9+16+21+24+25+24+21+16-2 = 154 (154 % 11 = 0)
        var validRsin = "123456782";

        // Act & Assert
        var exception = Record.Exception(() => RsinValidator.ValidateRsin(validRsin, _mockLogger.Object));
        Assert.Null(exception);
    }
}
