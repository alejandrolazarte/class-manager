using ClassManager.Core.Common;
using ClassManager.Core.Domain.Users;
using ClassManager.Records;
using ClassManager.Tenancy;

namespace ClassManager.Core.Domain.Instructors;

public sealed class Instructor : ITenantOwned, IAuditable
{
    private Instructor()
    {
    }

    public long Id { get; private set; }
    public long TenantId { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public long? CreatedByUserId { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
    public long? UpdatedByUserId { get; private set; }
    public bool IsActive { get; private set; }
    public long UserId { get; private set; }
    public User User { get; private set; } = null!;

    public static Result<Instructor> Create(string? fullName, DateTimeOffset createdAt, string? email = null)
    {
        var person = User.Create(fullName, email, createdAt);
        if (person.IsFailure)
        {
            return person.Error!;
        }

        return new Instructor
        {
            IsActive = true,
            UserId = person.Value!.Id,
            User = person.Value,
        };
    }

    public Result Update(string? fullName, string? email)
    {
        var rename = User.Rename(fullName);
        return rename.IsFailure ? rename : User.ChangeEmail(email);
    }

    public void ChangePerson(User person)
    {
        UserId = person.Id;
        User = person;
    }

    public void Activate() => IsActive = true;

    public void Deactivate() => IsActive = false;
}
