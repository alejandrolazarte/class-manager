using System.Collections.Frozen;
using System.Globalization;
using System.Text.RegularExpressions;

using ClassManager.Core.Common;
using ClassManager.Core.Domain.Countries;
using ClassManager.Core.Domain.Organizations;

namespace ClassManager.Core.Domain.Businesses;

public sealed partial class Business
{
    public const int NameMinLength = 2;
    public const int NameMaxLength = 120;
    public const int SlugMinLength = 3;
    public const int SlugMaxLength = 80;
    public const int TimeZoneIdMaxLength = 64;
    public const int CurrencyCodeLength = 3;
    public const int DefaultCountryCallingCodeMaxLength = Country.CallingCodeMaxLength;
    public const int BrandColorLength = 7;
    public const int LocationNameMaxLength = 200;
    public const int LocationMapsUrlMaxLength = 500;

    private const string NameLengthMessage = "Name must be between 2 and 120 characters.";
    private const string SlugFormatMessage = "Slug must be 3 to 80 lowercase letters, digits or hyphens.";
    private const string CountryUnknownMessage = "Country is not supported.";
    private const string TimeZoneIdUnknownMessage = "Time zone id is not a time zone of the country.";
    private const string CurrencyCodeUnknownMessage = "Currency code is not a known ISO 4217 code.";
    private const string BrandNameLengthMessage = "Brand name must be between 2 and 120 characters.";
    private const string BrandColorFormatMessage = "Brand colors must be # followed by 6 hexadecimal digits.";
    private const string LocationNameLengthMessage = "Location name must be at most 200 characters.";
    private const string LocationNameRequiredMessage = "Location name is required when there is a Google Maps link.";
    private const string LocationMapsUrlMessage = "The location link must be a Google Maps https link of at most 500 characters.";

    private static readonly Lazy<FrozenSet<string>> KnownCurrencyCodes = new(LoadKnownCurrencyCodes);

    private Business()
    {
    }

    public long Id { get; private set; }
    public long OrganizationId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Slug { get; private set; } = string.Empty;
    public string CountryCode { get; private set; } = string.Empty;
    public Country Country { get; private set; } = null!;
    public Organization? Organization { get; private set; }
    public string TimeZoneId { get; private set; } = string.Empty;
    public string CurrencyCode { get; private set; } = string.Empty;
    public string DefaultCountryCallingCode { get; private set; } = string.Empty;
    public bool NoticedAbsencesKeepStreak { get; private set; } = true;
    public string? BrandName { get; private set; }
    public string? ThemeColor { get; private set; }
    public string? AccentColor { get; private set; }
    public DateTimeOffset? LogoUpdatedAt { get; private set; }
    public string? LocationName { get; private set; }
    public string? LocationMapsUrl { get; private set; }
    public string BrandDisplayName => BrandName ?? Name;
    public DateTimeOffset CreatedAt { get; private set; }

    public static Result<Business> CreateInNewOrganization(
        Organization organization,
        string? name,
        string? slug,
        Country? country,
        string? timeZoneId,
        string? currencyCode,
        DateTimeOffset createdAt)
    {
        var business = Create(organization.Id, name, slug, country, timeZoneId, currencyCode, createdAt);
        if (business.IsSuccess)
        {
            business.Value!.Organization = organization;
        }

        return business;
    }

    public static Result<Business> Create(
        long organizationId,
        string? name,
        string? slug,
        Country? country,
        string? timeZoneId,
        string? currencyCode,
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

        if (ValidateLocalization(country, timeZoneId, normalizedCurrencyCode) is { } localizationError)
        {
            return localizationError;
        }

        return new Business
        {
            OrganizationId = organizationId,
            Name = trimmedName,
            Slug = slug,
            CountryCode = country!.Code,
            Country = country,
            TimeZoneId = timeZoneId!,
            CurrencyCode = normalizedCurrencyCode!,
            DefaultCountryCallingCode = country.CallingCode,
            CreatedAt = createdAt.ToUniversalTime(),
        };
    }

    public Result UpdateSettings(string? name, Country? country, string? timeZoneId, string? currencyCode)
    {
        var trimmedName = name?.Trim() ?? string.Empty;
        var normalizedCurrencyCode = NormalizeCurrencyCode(currencyCode);
        var error = ValidateName(trimmedName)
            ?? ValidateLocalization(country, timeZoneId, normalizedCurrencyCode);
        if (error is not null)
        {
            return Result.Failure(error);
        }

        Name = trimmedName;
        CountryCode = country!.Code;
        Country = country;
        TimeZoneId = timeZoneId!;
        CurrencyCode = normalizedCurrencyCode!;
        DefaultCountryCallingCode = country.CallingCode;
        return Result.Success();
    }

    public Result UpdateBrand(string? brandName, string? themeColor, string? accentColor)
    {
        var trimmedBrandName = string.IsNullOrWhiteSpace(brandName) ? null : brandName.Trim();
        if (trimmedBrandName is { Length: < NameMinLength or > NameMaxLength })
        {
            return Result.Failure(ValidationError(BrandNameLengthMessage, nameof(BrandName)));
        }

        if (!IsBrandColor(themeColor))
        {
            return Result.Failure(ValidationError(BrandColorFormatMessage, nameof(ThemeColor)));
        }

        if (!IsBrandColor(accentColor))
        {
            return Result.Failure(ValidationError(BrandColorFormatMessage, nameof(AccentColor)));
        }

        BrandName = trimmedBrandName;
        ThemeColor = themeColor?.ToLowerInvariant();
        AccentColor = accentColor?.ToLowerInvariant();
        return Result.Success();
    }

    public Result ChangeLocation(string? locationName, string? locationMapsUrl)
    {
        var trimmedName = string.IsNullOrWhiteSpace(locationName) ? null : locationName.Trim();
        var trimmedMapsUrl = string.IsNullOrWhiteSpace(locationMapsUrl) ? null : locationMapsUrl.Trim();
        if (trimmedName is { Length: > LocationNameMaxLength })
        {
            return Result.Failure(ValidationError(LocationNameLengthMessage, nameof(LocationName)));
        }

        if (trimmedMapsUrl is not null && (trimmedMapsUrl.Length > LocationMapsUrlMaxLength || !GoogleMapsLink.IsValid(trimmedMapsUrl)))
        {
            return Result.Failure(ValidationError(LocationMapsUrlMessage, nameof(LocationMapsUrl)));
        }

        if (trimmedMapsUrl is not null && trimmedName is null)
        {
            return Result.Failure(ValidationError(LocationNameRequiredMessage, nameof(LocationName)));
        }

        LocationName = trimmedName;
        LocationMapsUrl = trimmedMapsUrl;
        return Result.Success();
    }

    public void ChangeLogo(DateTimeOffset updatedAt) => LogoUpdatedAt = updatedAt.ToUniversalTime();

    public void RemoveLogo() => LogoUpdatedAt = null;

    public void ChangeNoticedAbsencesKeepStreak(bool keepsStreak) => NoticedAbsencesKeepStreak = keepsStreak;

    public TimeZoneInfo FindTimeZone() => TimeZoneInfo.FindSystemTimeZoneById(TimeZoneId);

    public DateOnly TodayAt(DateTimeOffset now) => DateOnly.FromDateTime(LocalTimeAt(now));

    public DateTime LocalTimeAt(DateTimeOffset now) => TimeZoneInfo.ConvertTime(now, FindTimeZone()).DateTime;

    private static string? NormalizeCurrencyCode(string? currencyCode) => currencyCode?.Trim().ToUpperInvariant();

    private static ResultError? ValidateName(string trimmedName) =>
        trimmedName.Length is < NameMinLength or > NameMaxLength
            ? ValidationError(NameLengthMessage, nameof(Name))
            : null;

    private static ResultError? ValidateLocalization(Country? country, string? timeZoneId, string? normalizedCurrencyCode)
    {
        if (country is null)
        {
            return ValidationError(CountryUnknownMessage, nameof(CountryCode));
        }

        if (timeZoneId is null
            || !country.HasTimeZone(timeZoneId)
            || !TimeZoneInfo.TryFindSystemTimeZoneById(timeZoneId, out _))
        {
            return ValidationError(TimeZoneIdUnknownMessage, nameof(TimeZoneId));
        }

        if (normalizedCurrencyCode is null || !KnownCurrencyCodes.Value.Contains(normalizedCurrencyCode))
        {
            return ValidationError(CurrencyCodeUnknownMessage, nameof(CurrencyCode));
        }

        return null;
    }

    private static bool IsBrandColor(string? color) => color is null || BrandColorPattern().IsMatch(color);

    private static ResultError ValidationError(string message, string fieldName) =>
        new(Result.ValidationCode, message, ErrorKind.Validation) { FieldName = fieldName };

    private static FrozenSet<string> LoadKnownCurrencyCodes() =>
        CultureInfo.GetCultures(CultureTypes.SpecificCultures)
            .Select(culture => new RegionInfo(culture.Name).ISOCurrencySymbol)
            .Where(currencyCode => currencyCode.Length == CurrencyCodeLength)
            .ToFrozenSet(StringComparer.Ordinal);

    [GeneratedRegex("^[a-z0-9-]{3,80}$")]
    private static partial Regex SlugPattern();

    [GeneratedRegex("^#[0-9a-fA-F]{6}$")]
    private static partial Regex BrandColorPattern();
}
