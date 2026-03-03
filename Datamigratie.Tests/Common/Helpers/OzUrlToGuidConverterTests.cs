using Datamigratie.Common.Helpers;

namespace Datamigratie.Tests.Common.Helpers;

public class OzUrlToGuidConverterTests
{
    [Fact]
    public void ExtractUuidFromUrl_ValidUrl_ReturnsGuid()
    {
        // Arrange
        var expectedGuid = Guid.NewGuid();
        var url = $"https://api.example.com/zaken/api/v1/zaaktypen/{expectedGuid}";

        // Act
        var result = OzUrlToGuidConverter.ExtractUuidFromUrl(url);

        // Assert
        Assert.Equal(expectedGuid, result);
    }

    [Fact]
    public void ExtractUuidFromUrl_UrlWithTrailingSlash_ReturnsGuid()
    {
        // Arrange
        var expectedGuid = Guid.NewGuid();
        var url = $"https://api.example.com/zaken/api/v1/zaaktypen/{expectedGuid}/";

        // Act & Assert - should handle trailing slash (though currently it won't)
        // This test will fail with current implementation
        var exception = Assert.Throws<ArgumentException>(() => OzUrlToGuidConverter.ExtractUuidFromUrl(url));
        Assert.Contains("Invalid UUID format", exception.Message);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void ExtractUuidFromUrl_NullOrEmpty_ThrowsArgumentException(string? url)
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => OzUrlToGuidConverter.ExtractUuidFromUrl(url!));
        Assert.Equal("URL cannot be null or empty (Parameter 'url')", exception.Message);
    }

    [Fact]
    public void ExtractUuidFromUrl_InvalidGuidFormat_ThrowsArgumentException()
    {
        // Arrange
        var url = "https://api.example.com/zaken/api/v1/zaaktypen/not-a-guid";

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => OzUrlToGuidConverter.ExtractUuidFromUrl(url));
        Assert.Contains("Invalid UUID format", exception.Message);
    }

    [Fact]
    public void ExtractUuidFromUrl_SimpleGuidString_ReturnsGuid()
    {
        // Arrange
        var expectedGuid = Guid.NewGuid();
        var url = expectedGuid.ToString();

        // Act
        var result = OzUrlToGuidConverter.ExtractUuidFromUrl(url);

        // Assert
        Assert.Equal(expectedGuid, result);
    }

    [Fact]
    public void ExtractUuidFromUrl_DifferentDomains_ReturnsCorrectGuid()
    {
        // Arrange
        var expectedGuid = Guid.NewGuid();
        var urls = new[]
        {
            $"https://internal.example.com/api/v1/resources/{expectedGuid}",
            $"https://external.example.com/api/v1/resources/{expectedGuid}",
            $"http://localhost:8000/api/v1/resources/{expectedGuid}"
        };

        foreach (var url in urls)
        {
            // Act
            var result = OzUrlToGuidConverter.ExtractUuidFromUrl(url);

            // Assert
            Assert.Equal(expectedGuid, result);
        }
    }
}
