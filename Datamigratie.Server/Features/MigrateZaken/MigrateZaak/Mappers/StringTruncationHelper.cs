using System.Diagnostics.CodeAnalysis;

namespace Datamigratie.Server.Features.MigrateZaken.MigrateZaak.Mappers;

public static class StringTruncationHelper
{
    [return: NotNullIfNotNull(nameof(input))]
    public static string? TruncateWithDots(string? input, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(input) || input.Length <= maxLength)
            return input;

        var dots = "...";

        if (dots.Length >= maxLength)
        {
            return dots;
        }

        var truncatedInput = input[..(maxLength - dots.Length)].TrimEnd();

        return truncatedInput + dots;
    }
}
