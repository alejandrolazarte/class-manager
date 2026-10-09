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
    string? TimeZoneId,
    string? CurrencyCode,
    string? DefaultCountryCallingCode) : ICommand;

public sealed class CreateBranchUseCase(
    IBusinessRepository businessRepository,
    IBusinessMemberRepository businessMemberRepository,
    IInstructorRepository instructorRepository,
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
            command.TimeZoneId,
            command.CurrencyCode,
            command.DefaultCountryCallingCode,
            timeProvider.GetUtcNow());
        if (branch.IsFailure)
        {
            return branch.Error!;
        }

        var newBranch = branch.Value!;
        var owner = (await identityService.ListAccountsAsync([access.UserId], cancellationToken)).SingleOrDefault();
        businessRepository.Add(newBranch);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        if (owner is not null)
        {
            tenantScope.Establish(newBranch.Id);
            var ownerInstructor = Instructor.Create(owner.FullName).Value!;
            instructorRepository.Add(ownerInstructor);
            businessMemberRepository.Add(BusinessMember.CreateBranchOwner(newBranch.Id, access.UserId, ownerInstructor.Id));
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return new BranchResponse(
            newBranch.Id,
            newBranch.Name,
            owner is null ? null : BusinessRole.BranchOwner,
            IsBrandOwner: true,
            IsCurrent: false);
    }
}
