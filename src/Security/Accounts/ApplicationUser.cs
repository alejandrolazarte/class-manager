using Microsoft.AspNetCore.Identity;

namespace ClassManager.Security.Accounts;

public sealed class ApplicationUser : IdentityUser<Guid>
{
    public const int FullNameMaxLength = 120;
    public const int PreferredLanguageMaxLength = 10;
    public const string DefaultPreferredLanguage = "es";

    public string FullName { get; set; } = string.Empty;

    public DateOnly? BirthDate { get; set; }

    public string PreferredLanguage { get; set; } = DefaultPreferredLanguage;
}
