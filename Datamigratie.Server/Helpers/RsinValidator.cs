namespace Datamigratie.Server.Helpers;

/// <summary>
/// Validates RSIN (Rechtspersonen Samenwerkingsverbanden Informatienummer) numbers
/// </summary>
public static class RsinValidator
{
    /// <summary>
    /// Validates an RSIN number according to the 11-test
    /// </summary>
    /// <param name="rsin">The RSIN to validate</param>
    /// <returns>True if the RSIN is valid, false otherwise</returns>
    public static bool IsValid(string? rsin)
    {
        if (string.IsNullOrWhiteSpace(rsin))
        {
            return false;
        }

        // RSIN must be exactly 9 characters
        if (rsin.Length != 9)
        {
            return false;
        }

        // RSIN must contain only digits
        if (!rsin.All(char.IsDigit))
        {
            return false;
        }

        // Apply the 11-test (elfproef)
        return PassesElevenTest(rsin);
    }

    /// <summary>
    /// Applies the 11-test (elfproef) to validate the RSIN
    /// See: https://nl.wikipedia.org/wiki/Burgerservicenummer#11-proef
    /// </summary>
    private static bool PassesElevenTest(string number)
    {
        int sum = 0;

        // Multiply each digit by its position (9, 8, 7, 6, 5, 4, 3, 2, -1)
        for (int i = 0; i < 9; i++)
        {
            int digit = int.Parse(number[i].ToString());
            int multiplier = i == 8 ? -1 : (9 - i);
            sum += digit * multiplier;
        }

        // The sum must be divisible by 11
        return sum % 11 == 0;
    }

    /// <summary>
    /// Gets a validation error message for an invalid RSIN
    /// </summary>
    public static string GetValidationError(string? rsin)
    {
        if (string.IsNullOrWhiteSpace(rsin))
        {
            return "RSIN mag niet leeg zijn.";
        }

        if (rsin.Length != 9)
        {
            return "RSIN moet precies 9 cijfers bevatten.";
        }

        if (!rsin.All(char.IsDigit))
        {
            return "RSIN mag alleen cijfers bevatten.";
        }

        if (!PassesElevenTest(rsin))
        {
            return "RSIN is niet geldig volgens de 11-proef.";
        }

        return string.Empty;
    }
}
