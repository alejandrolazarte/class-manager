using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Common;

namespace ClassManager.Core.UseCases.Brands;

public sealed record GetBrandQuery;

public sealed class GetBrandUseCase(IBusinessRepository businessRepository) : IUseCase<GetBrandQuery, BrandResponse>
{
    public async Task<Result<BrandResponse>> ExecuteAsync(GetBrandQuery command, CancellationToken cancellationToken)
    {
        var business = await businessRepository.GetCurrentAsync(cancellationToken);

        return business is null ? BrandRules.BusinessNotFound<BrandResponse>() : BrandResponse.From(business);
    }
}
