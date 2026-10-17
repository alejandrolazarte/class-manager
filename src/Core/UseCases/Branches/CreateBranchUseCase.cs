using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Businesses;
using ClassManager.Core.Domain.Instructors;
using ClassManager.Core.UseCases.Businesses;
using ClassManager.Tenancy;

namespace ClassManager.Core.UseCases.Branches;

public sealed record CreateBranchCommand(
    string? Name,
    string? CountryCode,
    string? TimeZoneId,
    string? CurrencyCode) : ICommand;

public sealed class CreateBranchUseCase(
    IBusinessRepository businessRepository,
    ICountryRepository countryRepository,
    IBusinessMemberRepository businessMemberRepository,
    IInstructorRepository instructorRepository,
    IOrganizationMemberRepository organizationMemberRepository,
    IUserRepository userRepository,
    ICurrentMember currentMember,
    IIdentityService identityService,
    ITenantScope tenantScope,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider)
    : IUseCase<CreateBranchCommand, BranchResponse>
{
    public async Task<Result<BranchResponse>> ExecuteAsync(CreateBranchCommand command, CancellationToken cancellationToken)
    {
        var currentBusiness = await businessRepository.GetCurrentAsync(cancellationToken);
        var access = await currentMember.GetAccessAsync(cancellationToken);
        if (currentBusiness is null || access is null)
        {
            return Result.Unauthorized<BranchResponse>(BusinessErrorCodes.CurrentBusinessNotFoundMessage, BusinessErrorCodes.CurrentBusinessNotFound);
        }

        var slug = await BusinessSlugAllocator.FindAvailableAsync(businessRepository, command.Name, cancellationToken);
        var branch = Business.Create(
            currentBusiness.OrganizationId,
            command.Name,
            slug,
            await countryRepository.FindAsync(command.CountryCode, cancellationToken),
            command.TimeZoneId,
            command.CurrencyCode,
            timeProvider.GetUtcNow());
        if (branch.IsFailure)
        {
            return branch.Error!;
        }

        var newBranch = branch.Value!;
        var owner = (await identityService.ListAccountsAsync([access.AccountId], cancellationToken)).SingleOrDefault();
        businessRepository.Add(newBranch);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        if (owner is not null)
        {
            tenantScope.Establish(newBranch.Id);
            var ownerInstructor = Instructor.Create(owner.FullName, timeProvider.GetUtcNow()).Value!;
            ownerInstructor.User.LinkAccount(access.AccountId);
            instructorRepository.Add(ownerInstructor);
            businessMemberRepository.Add(BusinessMember.CreateBranchOwner(newBranch.Id, ownerInstructor.User, ownerInstructor.Id));
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }

        var brandOwnerAccountIds = await organizationMemberRepository.ListBrandOwnerUserIdsAsync(currentBusiness.OrganizationId, cancellationToken);
        var brandOwners = await identityService.ListAccountsAsync(brandOwnerAccountIds, cancellationToken);
        await userRepository.AddMissingInEveryBranchAsync(currentBusiness.OrganizationId, brandOwners, timeProvider.GetUtcNow(), cancellationToken);

        return new BranchResponse(
            newBranch.Id,
            newBranch.Name,
            owner is null ? null : BusinessRole.BranchOwner,
            IsBrandOwner: true,
            IsCurrent: false);
    }
}
