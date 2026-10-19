using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Audit;
using ClassManager.Core.Common;

namespace ClassManager.Core.UseCases.Brands;

public sealed record UpdateBrandCommand([property: AuditPlain] string? BrandName, [property: AuditPlain] string? ThemeColor, [property: AuditPlain] string? AccentColor) : ICommand;

public sealed class UpdateBrandUseCase(IBusinessRepository businessRepository, IUnitOfWork unitOfWork)
    : IUseCase<UpdateBrandCommand, BrandResponse>
{
    public async Task<Result<BrandResponse>> ExecuteAsync(UpdateBrandCommand command, CancellationToken cancellationToken)
    {
        var business = await businessRepository.GetCurrentForUpdateAsync(cancellationToken);
        if (business is null)
        {
            return BrandRules.BusinessNotFound<BrandResponse>();
        }

        var update = business.UpdateBrand(command.BrandName, command.ThemeColor, command.AccentColor);
        if (update.IsFailure)
        {
            return update.Error!;
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return BrandResponse.From(business);
    }
}
