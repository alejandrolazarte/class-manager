using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Subscriptions;
using ClassManager.Subscriptions.Access;

namespace ClassManager.Core.UseCases.Brands;

public sealed record GetBrandQuery : IQuery;

public sealed class GetBrandUseCase(IBusinessRepository businessRepository, IFeatureAccess featureAccess) : IUseCase<GetBrandQuery, BrandResponse>
{
    public async Task<Result<BrandResponse>> ExecuteAsync(GetBrandQuery command, CancellationToken cancellationToken)
    {
        var business = await businessRepository.GetCurrentAsync(cancellationToken);
        if (business is null)
        {
            return BrandRules.BusinessNotFound<BrandResponse>();
        }

        var brand = BrandResponse.From(business);
        var features = await featureAccess.GetCurrentAsync(cancellationToken);
        return features.Has(Features.Brand) ? brand : brand.WithoutPaidBranding();
    }
}
