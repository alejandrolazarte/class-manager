using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Subscriptions;
using ClassManager.Subscriptions.Catalog;
using ClassManager.Subscriptions.Subscribers;
using Microsoft.Extensions.Time.Testing;

namespace ClassManager.Backoffice.Core.U.Tests.Brands;

internal sealed class SubscriptionChangeScenario
{
    public const string Currency = "EUR";

    public static readonly DateTimeOffset Now = new(2026, 10, 20, 12, 0, 0, TimeSpan.Zero);

    public static readonly DateOnly Today = DateOnly.FromDateTime(Now.UtcDateTime);

    private readonly FakeTimeProvider _timeProvider = new(Now);

    public SubscriptionChangeScenario()
    {
        Repository.Plans.Add(Plan.Create(PlanCodes.Free, 0, 0m, Currency, BillingPeriod.Monthly, isDefault: true, durationInDays: 30));
        Repository.Plans.Add(Plan.Create(PlanCodes.Pro, 2, 29m, Currency, BillingPeriod.Monthly));
        Repository.Features.Add(Feature.CreateAddOn(Features.Branches, isCounted: true, 10m, Currency));
        Repository.Features.Add(Feature.CreateAddOn(Features.Shop, isCounted: false, 5m, Currency));
    }

    public long BrandId { get; } = TestIds.Next();

    public InMemoryBrandSubscriptionRepository Repository { get; } = new();

    public CountingUnitOfWork UnitOfWork { get; } = new();

    public Subscription WithCurrentSubscription(string planCode = PlanCodes.Free)
    {
        Repository.BrandIds.Add(BrandId);
        var subscription = Subscription.Start(BrandId, planCode, 0m, Currency, note: null, Now.AddDays(-10));
        Repository.Subscriptions.Add(subscription);
        return subscription;
    }

    public SubscriptionFeature WithAddOn(Subscription subscription, string featureCode, int? limit = null)
    {
        var addOn = SubscriptionFeature.Create(subscription.Id, featureCode, 10m, Currency, limit, expiredOn: null, Now.AddDays(-5));
        Repository.AddOns.Add(addOn);
        return addOn;
    }

    public Task<Result<BrandPlanChangedResponse>> ChangePlanAsync(string planCode, DateOnly? expiredOn = null) =>
        new ChangeBrandPlanUseCase(Repository, UnitOfWork, _timeProvider)
            .ExecuteAsync(new ChangeBrandPlanCommand(BrandId, planCode, 29m, Currency, "Precio acordado", expiredOn), CancellationToken.None);

    public Task<Result<BrandAddOnResponse>> GiveAddOnAsync(string featureCode, int? limit) =>
        new GiveBrandAddOnUseCase(Repository, UnitOfWork, _timeProvider)
            .ExecuteAsync(new GiveBrandAddOnCommand(BrandId, featureCode, 10m, Currency, limit, ExpiredOn: null), CancellationToken.None);

    public Task<Result<BrandAddOnResponse>> RemoveAddOnAsync(long addOnId) =>
        new RemoveBrandAddOnUseCase(Repository, UnitOfWork, _timeProvider)
            .ExecuteAsync(new RemoveBrandAddOnCommand(BrandId, addOnId), CancellationToken.None);

    public Task<Result<BrandAddOnResponse>> RestoreAddOnAsync(long addOnId) =>
        new RestoreBrandAddOnUseCase(Repository, UnitOfWork)
            .ExecuteAsync(new RestoreBrandAddOnCommand(BrandId, addOnId), CancellationToken.None);
}

internal sealed class InMemoryBrandSubscriptionRepository : IBrandSubscriptionRepository
{
    public List<long> BrandIds { get; } = [];

    public List<Plan> Plans { get; } = [];

    public List<Feature> Features { get; } = [];

    public List<Subscription> Subscriptions { get; } = [];

    public List<SubscriptionFeature> AddOns { get; } = [];

    public Task<bool> BrandExistsAsync(long brandId, CancellationToken cancellationToken) =>
        Task.FromResult(BrandIds.Contains(brandId));

    public Task<IReadOnlyList<Plan>> ListActivePlansAsync(CancellationToken cancellationToken) =>
        Task.FromResult<IReadOnlyList<Plan>>(Plans);

    public Task<IReadOnlyList<Feature>> ListFeaturesAsync(CancellationToken cancellationToken) =>
        Task.FromResult<IReadOnlyList<Feature>>(Features);

    public Task<Subscription?> FindCurrentSubscriptionAsync(long brandId, CancellationToken cancellationToken) =>
        Task.FromResult(Subscriptions.FirstOrDefault(subscription => subscription.SubscriberId == brandId && !subscription.IsDeleted));

    public Task<IReadOnlyList<SubscriptionFeature>> ListAddOnsIncludingRemovedAsync(long subscriptionId, CancellationToken cancellationToken) =>
        Task.FromResult<IReadOnlyList<SubscriptionFeature>>([.. AddOns.Where(addOn => addOn.SubscriptionId == subscriptionId)]);

    public void Add(Subscription subscription) => Subscriptions.Add(subscription);

    public void Add(SubscriptionFeature addOn) => AddOns.Add(addOn);
}

internal sealed class CountingUnitOfWork : IUnitOfWork
{
    public int SaveCount { get; private set; }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        SaveCount++;
        return Task.CompletedTask;
    }

    public Task<IUnitOfWorkTransaction> BeginTransactionAsync(CancellationToken cancellationToken) =>
        throw new NotSupportedException();
}
