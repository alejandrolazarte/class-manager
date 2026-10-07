using Microsoft.AspNetCore.Identity;

namespace ClassManager.Security.Accounts;

public sealed class ApplicationUser : IdentityUser<Guid>
{
    public const int FullNameMaxLength = 120;

    public string FullName { get; set; } = string.Empty;

    public DateOnly? BirthDate { get; set; }
}
