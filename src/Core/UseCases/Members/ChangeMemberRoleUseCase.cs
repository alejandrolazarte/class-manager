using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Core.UseCases.Members;

public sealed record ChangeMemberRoleRequest(BusinessRole? Role, Guid? InstructorId);

public sealed record ChangeMemberRoleCommand(Guid MemberId, BusinessRole? Role, Guid? InstructorId);

public sealed class ChangeMemberRoleUseCase(
    IBusinessRepository businessRepository,
    IBusinessMemberRepository businessMemberRepository,
    IOrganizationMemberRepository organizationMemberRepository,
    IInstructorRepository instructorRepository,
    IIdentityService identityService,
    ICurrentMember currentMember,
    IUnitOfWork unitOfWork)
    : IUseCase<ChangeMemberRoleCommand, MemberResponse>
{
    private const string RoleRequiredMessage = "Choose a role.";

    public async Task<Result<MemberResponse>> ExecuteAsync(ChangeMemberRoleCommand command, CancellationToken cancellationToken)
    {
        if (command.Role is not { } role)
        {
            return Result.Validation<MemberResponse>(RoleRequiredMessage, fieldName: nameof(ChangeMemberRoleCommand.Role));
        }

        var member = await businessMemberRepository.GetForUpdateAsync(command.MemberId, cancellationToken);
        if (member is null)
        {
            return MemberRules.MemberNotFound();
        }

        var access = await currentMember.GetAccessAsync(cancellationToken);
        if (member.UserId == access?.UserId)
        {
            return MemberRules.OwnMembership();
        }

        if (!await MemberRules.CanManageRoleAsync(currentMember, member.Role, cancellationToken)
            || !await MemberRules.CanManageRoleAsync(currentMember, role, cancellationToken))
        {
            return MemberRules.RoleNotAllowed();
        }

        var instructorError = await MemberRules.ValidateInstructorAsync(
            instructorRepository, businessMemberRepository, command.InstructorId, member.Id, cancellationToken);
        if (instructorError is not null)
        {
            return instructorError;
        }

        var change = member.ChangeRole(role, command.InstructorId);
        if (change.IsFailure)
        {
            return change.Error!;
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        var accounts = await identityService.ListAccountsAsync([member.UserId], cancellationToken);
        var business = await businessRepository.GetCurrentAsync(cancellationToken);
        var isBrandOwner = business is not null
            && await organizationMemberRepository.FindForUpdateAsync(business.OrganizationId, member.UserId, cancellationToken) is not null;
        return MemberResponse.From(member, accounts.Count > 0 ? accounts[0] : null, access?.UserId, isBrandOwner);
    }
}
