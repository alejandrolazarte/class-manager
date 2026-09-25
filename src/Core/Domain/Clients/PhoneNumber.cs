using ClassManager.Core.Common;

namespace ClassManager.Core.Domain.Clients;

public sealed record PhoneNumber
{
    public const int MaxLength = 20;
    public const int MinimumDigitCount = 8;
    public const int MaximumDigitCount = 15;

    private const string InternationalPrefix = "+";
    private const string InternationalDialingPrefix = "00";
    private const char TrunkPrefix = '0';
    private const string RequiredMessage = "Phone number is required.";
    private const string InvalidFormatMessage = "Phone number may only contain digits, spaces, dashes, dots, parentheses and a leading '+'.";
    private const string InvalidLengthMessage = "Phone number must have between 8 and 15 digits including the country code.";

    private static readonly char[] SeparatorCharacters = [' ', '-', '.', '(', ')'];

    private PhoneNumber(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static Result<PhoneNumber> Create(string? rawValue, string defaultCountryCallingCode)
    {
        if (string.IsNullOrWhiteSpace(rawValue))
        {
            return Result.Validation<PhoneNumber>(RequiredMessage);
        }

        var digits = ToInternationalDigits(rawValue, defaultCountryCallingCode);
        if (digits is null)
        {
            return Result.Validation<PhoneNumber>(InvalidFormatMessage);
        }

        if (digits.Length is < MinimumDigitCount or > MaximumDigitCount)
        {
            return Result.Validation<PhoneNumber>(InvalidLengthMessage);
        }

        return new PhoneNumber(InternationalPrefix + digits);
    }

    public static PhoneNumber FromNormalized(string normalizedValue) => new(normalizedValue);

    public static string? ToNormalizedPrefix(string? rawValue, string defaultCountryCallingCode)
    {
        if (string.IsNullOrWhiteSpace(rawValue))
        {
            return null;
        }

        var digits = ToInternationalDigits(rawValue, defaultCountryCallingCode);
        return digits is null ? null : InternationalPrefix + digits;
    }

    public override string ToString() => Value;

    private static string? ToInternationalDigits(string rawValue, string defaultCountryCallingCode)
    {
        var compactValue = string.Concat(rawValue.Trim().Split(SeparatorCharacters, StringSplitOptions.RemoveEmptyEntries));

        string digits;
        if (compactValue.StartsWith(InternationalPrefix, StringComparison.Ordinal))
        {
            digits = compactValue[InternationalPrefix.Length..];
        }
        else if (compactValue.StartsWith(InternationalDialingPrefix, StringComparison.Ordinal))
        {
            digits = compactValue[InternationalDialingPrefix.Length..];
        }
        else
        {
            digits = defaultCountryCallingCode + compactValue.TrimStart(TrunkPrefix);
        }

        return digits.Length > 0 && digits.All(char.IsAsciiDigit) ? digits : null;
    }
}
