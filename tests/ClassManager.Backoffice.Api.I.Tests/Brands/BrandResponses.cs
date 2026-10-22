namespace ClassManager.Backoffice.Api.I.Tests.Brands;

internal sealed record BrandListResponse(IReadOnlyList<BrandSummaryResponse> Brands);

internal sealed record BrandSummaryResponse(long Id, string Name, string? OwnerEmail, string? PlanCode, string Status, int BranchCount, int StudentCount);

internal sealed record BrandDetailResponse(
    long Id,
    string Name,
    string Status,
    IReadOnlyList<BrandOwnerResponse> Owners,
    BrandSubscriptionResponse? Subscription,
    IReadOnlyList<BrandFeatureResponse> Features,
    IReadOnlyList<BrandBranchResponse> Branches,
    IReadOnlyList<BrandAddOnResponse> AddOns,
    IReadOnlyList<BrandSubscriptionResponse> PastSubscriptions);

internal sealed record BrandOwnerResponse(string FullName, string? Email);

internal sealed record BrandSubscriptionResponse(string PlanCode, decimal Price, string Currency);

internal sealed record BrandBranchResponse(string Name, int StudentCount);

internal sealed record BrandFeatureResponse(string Code, int? Limit);

internal sealed record BrandAddOnResponse(long Id, string FeatureCode, int? Limit);
