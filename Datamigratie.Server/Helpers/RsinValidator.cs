namespace Datamigratie.Server.Helpers;

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
            throw new ArgumentException("RSIN mag niet leeg zijn.");
        }

        if (rsin.Length != 9)
        {
            throw new ArgumentException("RSIN moet precies 9 cijfers bevatten.");
        }

        if (!rsin.All(char.IsDigit))
        {
            throw new ArgumentException("RSIN mag alleen cijfers bevatten.");
        }

        if (!PassesElevenTest(rsin))
        {
            throw new ArgumentException("RSIN is niet geldig volgens de 11-proef.");
        }
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
