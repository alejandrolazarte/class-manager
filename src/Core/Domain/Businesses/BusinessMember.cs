using ClassManager.Core.Common;
using ClassManager.Core.Domain.Users;
using ClassManager.Records;
using ClassManager.Tenancy;

namespace ClassManager.Core.Domain.Businesses;

public sealed class BusinessMember : ITenantOwned, IAuditable, ISoftDeletable
{
    public const int RoleMaxLength = 20;

    private const string InstructorRequiredMessage = "This role only sees its own classes, so it needs a linked instructor.";

    private BusinessMember()
    {
    }

    public long Id { get; private set; }
    public long TenantId { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public long? CreatedByUserId { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
    public long? UpdatedByUserId { get; private set; }
    public long UserId { get; private set; }
    public User User { get; private set; } = null!;
    public BusinessRole Role { get; private set; }
    public long? CustomRoleId { get; private set; }
    public long? InstructorId { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTimeOffset? DeletedAt { get; private set; }
    public long? DeletedByUserId { get; private set; }

    public static BusinessMember CreateBranchOwner(long businessId, User user, long? instructorId = null) =>
        new()
        {
            TenantId = businessId,
            UserId = user.Id,
            User = user,
            Role = BusinessRole.BranchOwner,
            InstructorId = instructorId,
        };

    public static Result<BusinessMember> Create(long businessId, User user, MemberRole role, long? instructorId)
    {
        if (ValidateRole(role, instructorId) is { } error)
        {
            return error;
        }

        return new BusinessMember
        {
            TenantId = businessId,
            UserId = user.Id,
            User = user,
            Role = role.Role,
            CustomRoleId = role.CustomRoleId,
            InstructorId = instructorId,
        };
    }

    public Result ChangeRole(MemberRole role, long? instructorId)
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

    public static ResultError? ValidateRole(MemberRole role, long? instructorId) =>
        instructorId is null && role.NeedsInstructor
            ? new ResultError(MemberErrorCodes.InstructorRequired, InstructorRequiredMessage, ErrorKind.Validation) { FieldName = nameof(InstructorId) }
            : null;

    public void Delete(DateTimeOffset deletedAt)
    {
        DeletedAt = SoftDeleteGuard.Delete(DeletedAt, deletedAt);
        IsDeleted = true;
    }
}
