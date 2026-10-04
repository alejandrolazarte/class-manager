using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Core.UseCases.Members;

public sealed record ChangeMemberRoleRequest(BusinessRole? Role, Guid? InstructorId, Guid? CustomRoleId = null);

public sealed record ChangeMemberRoleCommand(Guid MemberId, BusinessRole? Role, Guid? InstructorId, Guid? CustomRoleId = null) : ICommand;

public sealed class ChangeMemberRoleUseCase(
    IBusinessRepository businessRepository,
    IBusinessMemberRepository businessMemberRepository,
    IOrganizationMemberRepository organizationMemberRepository,
    IInstructorRepository instructorRepository,
    ICustomRoleRepository customRoleRepository,
    IIdentityService identityService,
    ICurrentMember currentMember,
    IUnitOfWork unitOfWork)
    : IUseCase<ChangeMemberRoleCommand, MemberResponse>
{
    public async Task<Result<MemberResponse>> ExecuteAsync(ChangeMemberRoleCommand command, CancellationToken cancellationToken)
    {
        var role = await MemberRules.ResolveRoleAsync(customRoleRepository, command.Role, command.CustomRoleId, cancellationToken);
        if (role.IsFailure)
        {
            return role.Error!;
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

        if (!await MemberRules.CanManageRoleAsync(currentMember, member.Role, cancellationToken))
        {
            return MemberRules.RoleNotAllowed();
        }

        if (await MemberRules.CheckCanGiveAsync(currentMember, role.Value!, cancellationToken) is { } roleError)
        {
            return roleError;
        }

        var instructorError = await MemberRules.ValidateInstructorAsync(
            instructorRepository, businessMemberRepository, command.InstructorId, member.Id, cancellationToken);
        if (instructorError is not null)
        {
            return instructorError;
        }

        var change = member.ChangeRole(role.Value!, command.InstructorId);
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
