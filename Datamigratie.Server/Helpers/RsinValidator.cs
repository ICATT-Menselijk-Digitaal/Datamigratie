namespace Datamigratie.Server.Helpers;

using System.Diagnostics.CodeAnalysis;

/// <summary>
/// Validates RSIN (Rechtspersonen Samenwerkingsverbanden Informatienummer) numbers
/// </summary>
public static class RsinValidator
{
    /// <summary>
    /// Validates an RSIN number according to the 11-test and throws ArgumentException if invalid
    /// </summary>
    /// <param name="rsin">The RSIN to validate</param>
    /// <param name="logger">Logger for logging validation errors</param>
    /// <exception cref="ArgumentException">Thrown when RSIN is invalid</exception>
    public static void ValidateRsin(string? rsin, ILogger logger)
    {
        if (string.IsNullOrWhiteSpace(rsin))
        {
            LogAndThrow(logger, "RSIN mag niet leeg zijn.", rsin);
        }

        if (rsin.Length != 9)
        {
            LogAndThrow(logger, "RSIN moet precies 9 cijfers bevatten.", rsin);
        }

        if (!rsin.All(char.IsDigit))
        {
            LogAndThrow(logger, "RSIN mag alleen cijfers bevatten.", rsin);
        }

        if (!PassesElevenTest(rsin))
        {
            LogAndThrow(logger, "RSIN is niet geldig volgens de 11-proef.", rsin);
        }
    }

    [DoesNotReturn]
    private static void LogAndThrow(ILogger logger, string message, string? rsin)
    {
        logger.LogWarning("RSIN validation failed for {Rsin}: {Message}", rsin, message);
        throw new ArgumentException(message, nameof(rsin));
    }

    /// <summary>
    /// Applies the 11-test (elfproef) to validate the RSIN
    /// See: https://nl.wikipedia.org/wiki/Burgerservicenummer#11-proef
    /// </summary>
    private static bool PassesElevenTest(string number)
    {
        var sum = 0;

        // Multiply each digit by its position (9, 8, 7, 6, 5, 4, 3, 2, -1)
        for (var i = 0; i < 9; i++)
        {
            var digit = int.Parse(number[i].ToString());
            var multiplier = i == 8 ? -1 : 9 - i;
            sum += digit * multiplier;
        }

        // The sum must be divisible by 11
        return sum % 11 == 0;
    }
}
