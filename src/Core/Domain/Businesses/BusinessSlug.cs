using System.Globalization;
using System.Text;

namespace ClassManager.Core.Domain.Businesses;

public static class BusinessSlug
{
    public const string Fallback = "business";

    private const char Separator = '-';

    public static string FromName(string? name)
    {
        var decomposedName = (name ?? string.Empty).Normalize(NormalizationForm.FormD);
        var slug = new StringBuilder(decomposedName.Length);
        var lastAppendedIsSeparator = true;

        foreach (var character in decomposedName)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(character) == UnicodeCategory.NonSpacingMark)
            {
                continue;
            }

            var lowercaseCharacter = char.ToLowerInvariant(character);
            if (char.IsAsciiLetterOrDigit(lowercaseCharacter))
            {
                slug.Append(lowercaseCharacter);
                lastAppendedIsSeparator = false;
            }
            else if (!lastAppendedIsSeparator)
            {
                slug.Append(Separator);
                lastAppendedIsSeparator = true;
            }
        }

        var trimmedSlug = TruncateToLength(slug.ToString(), Business.SlugMaxLength);
        if (trimmedSlug.Length == 0)
        {
            return Fallback;
        }

        return trimmedSlug.Length < Business.SlugMinLength
            ? Fallback + Separator + trimmedSlug
            : trimmedSlug;
    }

    public static string WithSuffix(string baseSlug, int suffix)
    {
        var suffixText = Separator + suffix.ToString(CultureInfo.InvariantCulture);
        return TruncateToLength(baseSlug, Business.SlugMaxLength - suffixText.Length) + suffixText;
    }

    private static string TruncateToLength(string slug, int maximumLength) =>
        (slug.Length > maximumLength ? slug[..maximumLength] : slug).Trim(Separator);
}
