using ClassManager.Backoffice.Core.Brands;
using ClassManager.Core.Common;
using ClassManager.Core.UseCases;

namespace ClassManager.Backoffice.Core.UseCases;

public sealed record ListExpiringAddOnsQuery : IQuery;

public sealed record ExpiringAddOnResponse(long BrandId, string BrandName, string FeatureCode, DateOnly ExpiredOn);

public sealed record ExpiringAddOnListResponse(IReadOnlyList<ExpiringAddOnResponse> AddOns);

public sealed class ListExpiringAddOnsUseCase(IBrandDirectory brandDirectory, TimeProvider timeProvider)
    : IUseCase<ListExpiringAddOnsQuery, ExpiringAddOnListResponse>
{
    public const int DaysAhead = 7;

    public async Task<Result<ExpiringAddOnListResponse>> ExecuteAsync(ListExpiringAddOnsQuery command, CancellationToken cancellationToken)
    {
        var today = DateOnly.FromDateTime(timeProvider.GetUtcNow().UtcDateTime);
        var addOns = await brandDirectory.ListAddOnsExpiringBetweenAsync(today, today.AddDays(DaysAhead), cancellationToken);
        return new ExpiringAddOnListResponse([.. addOns
            .OrderBy(addOn => addOn.ExpiredOn)
            .ThenBy(addOn => addOn.BrandName, StringComparer.CurrentCultureIgnoreCase)
            .Select(addOn => new ExpiringAddOnResponse(addOn.BrandId, addOn.BrandName, addOn.FeatureCode, addOn.ExpiredOn))]);
    }
}
