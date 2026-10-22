using ClassManager.Backoffice.Core.Brands;
using ClassManager.Core.Audit;
using ClassManager.Core.Common;
using ClassManager.Core.UseCases;

namespace ClassManager.Backoffice.Core.UseCases;

[AuditedQuery]
public sealed record ListBrandsQuery : IQuery;

public sealed record BrandSummaryResponse(
    long Id,
    string Name,
    string? OwnerName,
    string? OwnerEmail,
    string? PlanCode,
    BrandStatus Status,
    int? TrialDaysLeft,
    DateOnly? ExpiredOn,
    int BranchCount,
    int StudentCount,
    DateTimeOffset CreatedAt);

public sealed record BrandListResponse(IReadOnlyList<BrandSummaryResponse> Brands) : ICountedRows
{
    public int RowCount => Brands.Count;
}

public sealed class ListBrandsUseCase(IBrandDirectory brandDirectory, TimeProvider timeProvider)
    : IUseCase<ListBrandsQuery, BrandListResponse>
{
    public async Task<Result<BrandListResponse>> ExecuteAsync(ListBrandsQuery command, CancellationToken cancellationToken)
    {
        var today = DateOnly.FromDateTime(timeProvider.GetUtcNow().UtcDateTime);
        var brands = await brandDirectory.ListAsync(cancellationToken);

        return new BrandListResponse([.. brands
            .OrderBy(brand => brand.Name, StringComparer.CurrentCultureIgnoreCase)
            .Select(brand => new BrandSummaryResponse(
                brand.Id,
                brand.Name,
                brand.Owner?.FullName,
                brand.Owner?.Email,
                brand.CurrentSubscription?.PlanCode,
                BrandSubscriptionStatus.Of(brand.CurrentSubscription, today),
                BrandSubscriptionStatus.TrialDaysLeft(brand.CurrentSubscription, today),
                brand.CurrentSubscription?.ExpiredOn,
                brand.BranchCount,
                brand.StudentCount,
                brand.CreatedAt))]);
    }
}
