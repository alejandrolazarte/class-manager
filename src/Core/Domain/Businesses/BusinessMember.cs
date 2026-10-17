using ClassManager.Core.Common;
using ClassManager.Core.Domain.Users;
using ClassManager.Records;
using ClassManager.Tenancy;

namespace ClassManager.Core.Domain.Businesses;

public sealed class BusinessMember : ITenantOwned, ISoftDeletable
{
    public const int RoleMaxLength = 20;

    private const string InstructorRequiredMessage = "This role only sees its own classes, so it needs a linked instructor.";

    private BusinessMember()
    {
    }

    public Guid Id { get; private set; }
    public Guid TenantId { get; private set; }
    public Guid UserId { get; private set; }
    public User User { get; private set; } = null!;
    public BusinessRole Role { get; private set; }
    public Guid? CustomRoleId { get; private set; }
    public Guid? InstructorId { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTimeOffset? DeletedAt { get; private set; }
    public Guid? DeletedByUserId { get; private set; }

    public static BusinessMember CreateBranchOwner(Guid businessId, User user, Guid? instructorId = null) =>
        new()
        {
            Id = Guid.CreateVersion7(),
            TenantId = businessId,
            UserId = user.Id,
            User = user,
            Role = BusinessRole.BranchOwner,
            InstructorId = instructorId,
        };

    public static Result<BusinessMember> Create(Guid businessId, User user, MemberRole role, Guid? instructorId)
    {
        if (ValidateRole(role, instructorId) is { } error)
        {
            return error;
        }

        return new BusinessMember
        {
            Id = Guid.CreateVersion7(),
            TenantId = businessId,
            UserId = user.Id,
            User = user,
            Role = role.Role,
            CustomRoleId = role.CustomRoleId,
            InstructorId = instructorId,
        };
    }

    public Result ChangeRole(MemberRole role, Guid? instructorId)
    {
        if (ValidateRole(role, instructorId) is { } error)
        {
            return Result.Failure(error);
        }

        Role = role.Role;
        CustomRoleId = role.CustomRoleId;
        InstructorId = instructorId;
        return Result.Success();
    }

    public static ResultError? ValidateRole(MemberRole role, Guid? instructorId) =>
        instructorId is null && role.NeedsInstructor
            ? new ResultError(MemberErrorCodes.InstructorRequired, InstructorRequiredMessage, ErrorKind.Validation) { FieldName = nameof(InstructorId) }
            : null;

    public void Delete(DateTimeOffset deletedAt)
    {
        DeletedAt = SoftDeleteGuard.Delete(DeletedAt, deletedAt);
        IsDeleted = true;
    }
}
