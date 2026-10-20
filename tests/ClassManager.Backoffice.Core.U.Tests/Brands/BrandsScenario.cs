using ClassManager.Core.Common;
using ClassManager.Subscriptions.Catalog;
using ClassManager.Subscriptions.Subscribers;
using Microsoft.Extensions.Time.Testing;

namespace ClassManager.Backoffice.Core.U.Tests.Brands;

internal sealed class BrandsScenario
{
    public const string Currency = "EUR";

    public static readonly DateTimeOffset Now = new(2026, 10, 20, 12, 0, 0, TimeSpan.Zero);

    private readonly InMemoryBrandDirectory _directory = new();

    public Guid BrandId { get; } = Guid.CreateVersion7();

    public BrandsScenario WithListedBrand(Subscription? currentSubscription)
    {
        _directory.Listings.Add(new BrandListing(BrandId, "Marca", Now, new BrandOwner("Dueña", "owner@example.com"), currentSubscription, 1, 0));
        return this;
    }

    public BrandsScenario WithBrand(
        IReadOnlyList<Subscription> subscriptions,
        IReadOnlyList<PlanFeature>? planFeatures = null,
        IReadOnlyList<SubscriptionFeature>? addOns = null,
        IReadOnlyList<BrandBranch>? branches = null)
    {
        _directory.Records.Add(new BrandRecord(
            BrandId,
            "Marca",
            Now,
            [new BrandOwner("Dueña", "owner@example.com")],
            branches ?? [],
            subscriptions,
            addOns ?? [],
            planFeatures ?? []));
        return this;
    }

    public Subscription StartSubscription(string planCode, DateTimeOffset createdOn, decimal price = 0m) =>
        Subscription.Start(BrandId, planCode, price, Currency, note: null, createdOn);

    public async Task<BrandSummaryResponse> ListSingleAsync()
    {
        var result = await new ListBrandsUseCase(_directory, new FakeTimeProvider(Now))
            .ExecuteAsync(new ListBrandsQuery(), CancellationToken.None);
        return result.Value!.Brands.ShouldHaveSingleItem();
    }

    public Task<Result<BrandDetailResponse>> GetAsync(Guid? brandId = null) =>
        new GetBrandUseCase(_directory, new FakeTimeProvider(Now))
            .ExecuteAsync(new GetBrandQuery(brandId ?? BrandId), CancellationToken.None);
}

internal sealed class InMemoryBrandDirectory : IBrandDirectory
{
    public List<BrandListing> Listings { get; } = [];

    public List<BrandRecord> Records { get; } = [];

    public Task<IReadOnlyList<BrandListing>> ListAsync(CancellationToken cancellationToken) =>
        Task.FromResult<IReadOnlyList<BrandListing>>(Listings);

    public Task<BrandRecord?> FindAsync(Guid brandId, CancellationToken cancellationToken) =>
        Task.FromResult(Records.FirstOrDefault(record => record.Id == brandId));
}
