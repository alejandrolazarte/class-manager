using ClassManager.Core.Common;
using ClassManager.Core.Domain.Authorization;
using ClassManager.Tenancy;

namespace ClassManager.Core.Domain.Businesses;

public sealed class BusinessMember : ITenantOwned
{
    public const int RoleMaxLength = 20;

    private const string InstructorRequiredMessage = "This role only sees its own classes, so it needs a linked coach.";

    private BusinessMember()
    {
    }

    public Guid Id { get; private set; }
    public Guid TenantId { get; private set; }
    public Guid UserId { get; private set; }
    public BusinessRole Role { get; private set; }
    public Guid? InstructorId { get; private set; }

    public static BusinessMember CreateBranchOwner(Guid businessId, Guid userId) =>
        new()
        {
            Id = Guid.CreateVersion7(),
            TenantId = businessId,
            UserId = userId,
            Role = BusinessRole.BranchOwner,
        };

    public static Result<BusinessMember> Create(Guid businessId, Guid userId, BusinessRole role, Guid? instructorId)
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
            Role = role,
            InstructorId = instructorId,
        };
    }

    public Result ChangeRole(BusinessRole role, Guid? instructorId)
    {
        if (ValidateRole(role, instructorId) is { } error)
        {
            return Result.Failure(error);
        }

        Role = role;
        InstructorId = instructorId;
        return Result.Success();
    }

    public static ResultError? ValidateRole(BusinessRole role, Guid? instructorId) =>
        instructorId is null && SystemRolePermissions.NeedsInstructor(role)
            ? new ResultError(MemberErrorCodes.InstructorRequired, InstructorRequiredMessage, ErrorKind.Validation) { FieldName = nameof(InstructorId) }
            : null;
}
