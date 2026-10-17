using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Organizations;
using ClassManager.Core.UseCases.Members;

namespace ClassManager.Core.UseCases.Branches;

public sealed record SetBrandOwnerCommand(Guid MemberId, bool IsBrandOwner) : ICommand;

public sealed record BrandOwnerResponse(Guid MemberId, bool IsBrandOwner);

public sealed class SetBrandOwnerUseCase(
    IBusinessRepository businessRepository,
    IBusinessMemberRepository businessMemberRepository,
    IOrganizationMemberRepository organizationMemberRepository,
    IUserRepository userRepository,
    IIdentityService identityService,
    ICurrentMember currentMember,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider)
    : IUseCase<SetBrandOwnerCommand, BrandOwnerResponse>
{
    public async Task<Result<BrandOwnerResponse>> ExecuteAsync(SetBrandOwnerCommand command, CancellationToken cancellationToken)
    {
        var member = await businessMemberRepository.GetForUpdateAsync(command.MemberId, cancellationToken);
        var business = await businessRepository.GetCurrentAsync(cancellationToken);
        if (member is null || business is null)
        {
            return MemberRules.MemberNotFound();
        }

        var access = await currentMember.GetAccessAsync(cancellationToken);
        if (member.AccountId == access?.AccountId)
        {
            return MemberRules.OwnMembership();
        }

        var brandOwner = await organizationMemberRepository.FindForUpdateAsync(business.OrganizationId, member.AccountId, cancellationToken);
        if (command.IsBrandOwner && brandOwner is null)
        {
            organizationMemberRepository.Add(OrganizationMember.CreateBrandOwner(business.OrganizationId, member.AccountId));
        }
        else if (!command.IsBrandOwner && brandOwner is not null)
        {
            var brandOwnerUserIds = await organizationMemberRepository.ListBrandOwnerUserIdsAsync(business.OrganizationId, cancellationToken);
            if (brandOwnerUserIds.Count <= 1)
            {
                return Result.Conflict<BrandOwnerResponse>(BranchErrorCodes.LastBrandOwnerMessage, BranchErrorCodes.LastBrandOwner);
            }

            organizationMemberRepository.Remove(brandOwner);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        if (command.IsBrandOwner)
        {
            var account = await identityService.ListAccountsAsync([member.AccountId], cancellationToken);
            await userRepository.AddMissingInEveryBranchAsync(business.OrganizationId, account, timeProvider.GetUtcNow(), cancellationToken);
        }

        return new BrandOwnerResponse(member.Id, command.IsBrandOwner);
    }
}
