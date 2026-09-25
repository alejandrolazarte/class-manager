using System.Collections.Frozen;
using System.Globalization;
using System.Text.RegularExpressions;

using ClassManager.Core.Common;

namespace ClassManager.Core.Domain.Businesses;

public sealed partial class Business
{
    public const int NameMinLength = 2;
    public const int NameMaxLength = 120;
    public const int SlugMinLength = 3;
    public const int SlugMaxLength = 80;
    public const int TimeZoneIdMaxLength = 64;
    public const int CurrencyCodeLength = 3;
    public const int DefaultCountryCallingCodeMaxLength = 3;

    private const string NameLengthMessage = "Name must be between 2 and 120 characters.";
    private const string SlugFormatMessage = "Slug must be 3 to 80 lowercase letters, digits or hyphens.";
    private const string TimeZoneIdUnknownMessage = "Time zone id is not a known IANA time zone.";
    private const string CurrencyCodeUnknownMessage = "Currency code is not a known ISO 4217 code.";
    private const string DefaultCountryCallingCodeFormatMessage = "Default country calling code must be 1 to 3 digits.";

    private static readonly Lazy<FrozenSet<string>> KnownCurrencyCodes = new(LoadKnownCurrencyCodes);

    private Business()
    {
    }

    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Slug { get; private set; } = string.Empty;
    public string TimeZoneId { get; private set; } = string.Empty;
    public string CurrencyCode { get; private set; } = string.Empty;
    public string DefaultCountryCallingCode { get; private set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; private set; }
    public decimal? DefaultMonthlyFee { get; private set; }

    public static Result<Business> Create(
        string? name,
        string? slug,
        string? timeZoneId,
        string? currencyCode,
        string? defaultCountryCallingCode,
        DateTimeOffset createdAt)
    {
        var trimmedName = name?.Trim() ?? string.Empty;
        var normalizedCurrencyCode = NormalizeCurrencyCode(currencyCode);
        if (ValidateName(trimmedName) is { } nameError)
        {
            return nameError;
        }

        if (slug is null || !SlugPattern().IsMatch(slug))
        {
            return ValidationError(SlugFormatMessage, nameof(Slug));
        }

        if (ValidateLocalization(timeZoneId, normalizedCurrencyCode, defaultCountryCallingCode) is { } localizationError)
        {
            return localizationError;
        }

        return new Business
        {
            Id = Guid.CreateVersion7(),
            Name = trimmedName,
            Slug = slug,
            TimeZoneId = timeZoneId!,
            CurrencyCode = normalizedCurrencyCode!,
            DefaultCountryCallingCode = defaultCountryCallingCode!,
            CreatedAt = createdAt.ToUniversalTime(),
        };
    }

    public Result UpdateSettings(string? name, string? timeZoneId, string? currencyCode, string? defaultCountryCallingCode)
    {
        var trimmedName = name?.Trim() ?? string.Empty;
        var normalizedCurrencyCode = NormalizeCurrencyCode(currencyCode);
        var error = ValidateName(trimmedName)
            ?? ValidateLocalization(timeZoneId, normalizedCurrencyCode, defaultCountryCallingCode);
        if (error is not null)
        {
            return Result.Failure(error);
        }

        Name = trimmedName;
        TimeZoneId = timeZoneId!;
        CurrencyCode = normalizedCurrencyCode!;
        DefaultCountryCallingCode = defaultCountryCallingCode!;
        return Result.Success();
    }

    public Result SetDefaultMonthlyFee(decimal? amount)
    {
        var validation = Fees.MonthlyFee.Validate(amount, nameof(DefaultMonthlyFee));
        if (validation.IsFailure)
        {
            return validation;
        }

        DefaultMonthlyFee = amount;
        return Result.Success();
    }

    public TimeZoneInfo FindTimeZone() => TimeZoneInfo.FindSystemTimeZoneById(TimeZoneId);

    public DateOnly TodayAt(DateTimeOffset now) => DateOnly.FromDateTime(TimeZoneInfo.ConvertTime(now, FindTimeZone()).DateTime);

    private static string? NormalizeCurrencyCode(string? currencyCode) => currencyCode?.Trim().ToUpperInvariant();

    private static ResultError? ValidateName(string trimmedName) =>
        trimmedName.Length is < NameMinLength or > NameMaxLength
            ? ValidationError(NameLengthMessage, nameof(Name))
            : null;

    private static ResultError? ValidateLocalization(string? timeZoneId, string? normalizedCurrencyCode, string? defaultCountryCallingCode)
    {
        if (timeZoneId is null
            || timeZoneId.Length > TimeZoneIdMaxLength
            || !TimeZoneInfo.TryFindSystemTimeZoneById(timeZoneId, out _))
        {
            return ValidationError(TimeZoneIdUnknownMessage, nameof(TimeZoneId));
        }

        if (normalizedCurrencyCode is null || !KnownCurrencyCodes.Value.Contains(normalizedCurrencyCode))
        {
            return ValidationError(CurrencyCodeUnknownMessage, nameof(CurrencyCode));
        }

        if (defaultCountryCallingCode is null || !CountryCallingCodePattern().IsMatch(defaultCountryCallingCode))
        {
            return ValidationError(DefaultCountryCallingCodeFormatMessage, nameof(DefaultCountryCallingCode));
        }

        return null;
    }

    private static ResultError ValidationError(string message, string fieldName) =>
        new(Result.ValidationCode, message, ErrorKind.Validation) { FieldName = fieldName };

    private static FrozenSet<string> LoadKnownCurrencyCodes() =>
        CultureInfo.GetCultures(CultureTypes.SpecificCultures)
            .Select(culture => new RegionInfo(culture.Name).ISOCurrencySymbol)
            .Where(currencyCode => currencyCode.Length == CurrencyCodeLength)
            .ToFrozenSet(StringComparer.Ordinal);

    [GeneratedRegex("^[a-z0-9-]{3,80}$")]
    private static partial Regex SlugPattern();

    [GeneratedRegex("^[1-9][0-9]{0,2}$")]
    private static partial Regex CountryCallingCodePattern();
}
