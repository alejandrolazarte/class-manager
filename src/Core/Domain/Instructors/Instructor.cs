using ClassManager.Core.Common;
using ClassManager.Core.Domain.Users;
using ClassManager.Tenancy;

namespace ClassManager.Core.Domain.Instructors;

public sealed class Instructor : ITenantOwned
{
    private Instructor()
    {
    }

    public Guid Id { get; private set; }
    public Guid TenantId { get; private set; }
    public bool IsActive { get; private set; }
    public Guid UserId { get; private set; }
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
            Id = Guid.CreateVersion7(),
            IsActive = true,
            UserId = person.Value!.Id,
            User = person.Value,
        };
    }

    public static Instructor CreateFor(User person) =>
        new()
        {
            Id = Guid.CreateVersion7(),
            IsActive = true,
            UserId = person.Id,
            User = person,
        };

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
