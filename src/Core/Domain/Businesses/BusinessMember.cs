using ClassManager.Core.Common;
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
    public BusinessRole Role { get; private set; }
    public Guid? CustomRoleId { get; private set; }
    public Guid? InstructorId { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTimeOffset? DeletedOn { get; private set; }

    public static BusinessMember CreateBranchOwner(Guid businessId, Guid userId, Guid? instructorId = null) =>
        new()
        {
            Id = Guid.CreateVersion7(),
            TenantId = businessId,
            UserId = userId,
            Role = BusinessRole.BranchOwner,
            InstructorId = instructorId,
        };

    public static Result<BusinessMember> Create(Guid businessId, Guid userId, MemberRole role, Guid? instructorId)
    {
        if (ValidateRole(role, instructorId) is { } error)
        {
            return error;
        }

        return new BusinessMember
        {
            Id = Guid.CreateVersion7(),
            TenantId = businessId,
            UserId = userId,
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

    public void Delete(DateTimeOffset deletedOn)
    {
        DeletedOn = DeletedOnGuard.Delete(DeletedOn, deletedOn);
        IsDeleted = true;
    }
}
