using ClassManager.Core.Domain.Clients;
using ClassManager.Records;
using ClassManager.Tenancy;

namespace ClassManager.Core.Domain.Users;

public sealed class User : ITenantOwned, ISoftDeletable
{
    public const int FullNameMaxLength = 120;
    public const int EmailMaxLength = 254;

    private const string FullNameRequiredMessage = "A user needs a full name.";

    private User()
    {
    }

    public Guid Id { get; private set; }
    public Guid TenantId { get; private set; }
    public string FullName { get; private set; } = string.Empty;
    public string? Email { get; private set; }
    public PhoneNumber? PhoneNumber { get; private set; }
    public DateOnly? BirthDate { get; private set; }
    public Guid? AccountId { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTimeOffset? DeletedAt { get; private set; }
    public Guid? DeletedByUserId { get; private set; }

    public static User Create(Guid tenantId, string fullName, DateTimeOffset createdAt) =>
        new()
        {
            Id = Guid.CreateVersion7(),
            TenantId = tenantId,
            FullName = RequireFullName(fullName),
            CreatedAt = createdAt.ToUniversalTime(),
        };

    public void Rename(string fullName) => FullName = RequireFullName(fullName);

    public void ChangeEmail(string? email) => Email = email;

    public void ChangePhoneNumber(PhoneNumber? phoneNumber) => PhoneNumber = phoneNumber;

    public void ChangeBirthDate(DateOnly? birthDate) => BirthDate = birthDate;

    public void LinkAccount(Guid accountId) => AccountId = accountId;

    public void Delete(DateTimeOffset deletedAt)
    {
        DeletedAt = SoftDeleteGuard.Delete(DeletedAt, deletedAt);
        IsDeleted = true;
    }

    private static string RequireFullName(string fullName) =>
        string.IsNullOrWhiteSpace(fullName)
            ? throw new ArgumentException(FullNameRequiredMessage, nameof(fullName))
            : fullName.Trim();
}
