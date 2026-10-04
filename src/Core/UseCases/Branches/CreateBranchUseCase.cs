using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Businesses;
using ClassManager.Core.UseCases.Businesses;

namespace ClassManager.Core.UseCases.Branches;

public sealed record CreateBranchCommand(
    string? Name,
    string? TimeZoneId,
    string? CurrencyCode,
    string? DefaultCountryCallingCode) : ICommand;

public sealed class CreateBranchUseCase(
    IBusinessRepository businessRepository,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider)
    : IUseCase<CreateBranchCommand, BranchResponse>
{
    public async Task<Result<BranchResponse>> ExecuteAsync(CreateBranchCommand command, CancellationToken cancellationToken)
    {
        var currentBusiness = await businessRepository.GetCurrentAsync(cancellationToken);
        if (currentBusiness is null)
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

        businessRepository.Add(branch.Value!);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new BranchResponse(branch.Value!.Id, branch.Value.Name, null, IsBrandOwner: true, IsCurrent: false);
    }
}
