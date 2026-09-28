using ClassManager.Core.Common;

namespace ClassManager.Core.Domain.Organizations;

public sealed class Organization
{
    public const int NameMinLength = 2;
    public const int NameMaxLength = 120;

    private const string NameLengthMessage = "Name must be between 2 and 120 characters.";

    private Organization()
    {
    }

    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; private set; }

    public static Result<Organization> Create(string? name, DateTimeOffset createdAt)
    {
        var trimmedName = name?.Trim() ?? string.Empty;
        if (trimmedName.Length is < NameMinLength or > NameMaxLength)
        {
            return Result.Validation<Organization>(NameLengthMessage, fieldName: nameof(Name));
        }

        return new Organization
        {
            Id = Guid.CreateVersion7(),
            Name = trimmedName,
            CreatedAt = createdAt.ToUniversalTime(),
        };
    }
}
