using Datamigratie.Server.Helper;
using Microsoft.Extensions.Configuration;

namespace Datamigratie.Tests.Server.Helpers;

public class ConfigHelperTests
{
    [Fact]
    public void GetRequiredConfigValue_ExistingKey_ReturnsValue()
    {
        // Arrange
        var configData = new Dictionary<string, string?>
        {
            { "TestKey", "TestValue" }
        };
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configData)
            .Build();

        // Act
        var result = ConfigHelper.GetRequiredConfigValue(configuration, "TestKey");

        // Assert
        Assert.Equal("TestValue", result);
    }

    [Fact]
    public void GetRequiredConfigValue_MissingKey_ThrowsInvalidOperationException()
    {
        // Arrange
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection([])
            .Build();

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() =>
            ConfigHelper.GetRequiredConfigValue(configuration, "MissingKey"));
        Assert.Equal("Missing required configuration value for 'MissingKey'", exception.Message);
    }

    [Fact]
    public void GetRequiredConfigValue_EmptyValue_ThrowsInvalidOperationException()
    {
        // Arrange
        var configData = new Dictionary<string, string?>
        {
            { "EmptyKey", "" }
        };
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configData)
            .Build();

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() =>
            ConfigHelper.GetRequiredConfigValue(configuration, "EmptyKey"));
        Assert.Equal("Missing required configuration value for 'EmptyKey'", exception.Message);
    }

    [Fact]
    public void GetRequiredConfigValue_NestedKey_ReturnsValue()
    {
        // Arrange
        var configData = new Dictionary<string, string?>
        {
            { "Section:SubSection:Key", "NestedValue" }
        };
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configData)
            .Build();

        // Act
        var result = ConfigHelper.GetRequiredConfigValue(configuration, "Section:SubSection:Key");

        // Assert
        Assert.Equal("NestedValue", result);
    }

    [Fact]
    public void GetRequiredConfigValue_WhitespaceValue_ReturnsWhitespace()
    {
        // Arrange - Note: current implementation treats whitespace as valid
        var configData = new Dictionary<string, string?>
        {
            { "WhitespaceKey", "   " }
        };
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configData)
            .Build();

        // Act
        var result = ConfigHelper.GetRequiredConfigValue(configuration, "WhitespaceKey");

        // Assert
        Assert.Equal("   ", result);
    }
}
