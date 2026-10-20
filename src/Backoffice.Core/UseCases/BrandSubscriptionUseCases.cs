using ClassManager.Backoffice.Core.Brands;
using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Audit;
using ClassManager.Core.Common;
using ClassManager.Core.UseCases;
using ClassManager.Subscriptions.Catalog;
using ClassManager.Subscriptions.Subscribers;

namespace ClassManager.Backoffice.Core.UseCases;

public sealed record GetSubscriptionCatalogQuery : IQuery;

public sealed record CatalogPlanResponse(string Code, decimal? ListPrice, string Currency);

public sealed record CatalogFeatureResponse(string Code, bool IsCounted, bool IsAddOn, decimal? AddOnListPrice, string? Currency);

public sealed record SubscriptionCatalogResponse(IReadOnlyList<CatalogPlanResponse> Plans, IReadOnlyList<CatalogFeatureResponse> Features);

public sealed record ChangeBrandPlanCommand(
    Guid BrandId,
    [property: AuditPlain] string? PlanCode,
    decimal Price,
    [property: AuditPlain] string? Currency,
    [property: AuditPersonal] string? Note,
    DateOnly? ExpiredOn) : ICommand;

public sealed record BrandPlanChangedResponse(Guid SubscriptionId);

public sealed record GiveBrandAddOnCommand(
    Guid BrandId,
    [property: AuditPlain] string? FeatureCode,
    decimal Price,
    [property: AuditPlain] string? Currency,
    int? Limit,
    DateOnly? ExpiredOn) : ICommand;

public sealed record RemoveBrandAddOnCommand(Guid BrandId, Guid AddOnId) : ICommand;

public sealed record RestoreBrandAddOnCommand(Guid BrandId, Guid AddOnId) : ICommand;

public sealed class GetSubscriptionCatalogUseCase(IBrandSubscriptionRepository repository)
    : IUseCase<GetSubscriptionCatalogQuery, SubscriptionCatalogResponse>
{
    public async Task<Result<SubscriptionCatalogResponse>> ExecuteAsync(GetSubscriptionCatalogQuery command, CancellationToken cancellationToken)
    {
        var plans = await repository.ListActivePlansAsync(cancellationToken);
        var features = await repository.ListFeaturesAsync(cancellationToken);
        return new SubscriptionCatalogResponse(
            [.. plans.OrderBy(plan => plan.DisplayOrder).Select(plan => new CatalogPlanResponse(plan.Code, plan.ListPrice, plan.Currency))],
            [.. features.OrderBy(feature => feature.Code, StringComparer.Ordinal)
                .Select(feature => new CatalogFeatureResponse(feature.Code, feature.IsCounted, feature.IsAddOn, feature.AddOnListPrice, feature.Currency))]);
    }
}

public sealed class ChangeBrandPlanUseCase(IBrandSubscriptionRepository repository, IUnitOfWork unitOfWork, TimeProvider timeProvider)
    : IUseCase<ChangeBrandPlanCommand, BrandPlanChangedResponse>
{
    private const string PlanCodeField = "PlanCode";
    private const string NoteField = "Note";

    public async Task<Result<BrandPlanChangedResponse>> ExecuteAsync(ChangeBrandPlanCommand command, CancellationToken cancellationToken)
    {
        var now = timeProvider.GetUtcNow();
        if (!await repository.BrandExistsAsync(command.BrandId, cancellationToken))
        {
            return Result.NotFound<BrandPlanChangedResponse>(BrandErrorCodes.BrandNotFoundMessage, BrandErrorCodes.BrandNotFound);
        }

        var plans = await repository.ListActivePlansAsync(cancellationToken);
        var plan = plans.FirstOrDefault(candidate => candidate.Code == command.PlanCode?.Trim());
        if (plan is null)
        {
            return SubscriptionTermsValidation.Error(BrandErrorCodes.UnknownPlanMessage, BrandErrorCodes.UnknownPlan, PlanCodeField);
        }

        if (SubscriptionTermsValidation.Validate(command.Price, command.Currency, command.ExpiredOn, DateOnly.FromDateTime(now.UtcDateTime)) is { } termsError)
        {
            return termsError;
        }

        if (command.Note?.Trim() is { Length: > CatalogRules.NoteMaxLength })
        {
            return SubscriptionTermsValidation.Error(BrandErrorCodes.NoteTooLongMessage, BrandErrorCodes.NoteTooLong, NoteField);
        }

        var current = await repository.FindCurrentSubscriptionAsync(command.BrandId, cancellationToken);
        current?.Delete(now);
        var replacement = Subscription.Start(command.BrandId, plan.Code, command.Price, command.Currency!, command.Note, now);
        if (command.ExpiredOn is { } expiredOn)
        {
            replacement.Expire(expiredOn);
        }

        repository.Add(replacement);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return new BrandPlanChangedResponse(replacement.Id);
    }
}

public sealed class GiveBrandAddOnUseCase(IBrandSubscriptionRepository repository, IUnitOfWork unitOfWork, TimeProvider timeProvider)
    : IUseCase<GiveBrandAddOnCommand, BrandAddOnResponse>
{
    private const string FeatureCodeField = "FeatureCode";
    private const string LimitField = "Limit";

    public async Task<Result<BrandAddOnResponse>> ExecuteAsync(GiveBrandAddOnCommand command, CancellationToken cancellationToken)
    {
        var now = timeProvider.GetUtcNow();
        if (!await repository.BrandExistsAsync(command.BrandId, cancellationToken))
        {
            return Result.NotFound<BrandAddOnResponse>(BrandErrorCodes.BrandNotFoundMessage, BrandErrorCodes.BrandNotFound);
        }

        var features = await repository.ListFeaturesAsync(cancellationToken);
        var feature = features.FirstOrDefault(candidate => candidate.Code == command.FeatureCode?.Trim());
        if (feature is null)
        {
            return SubscriptionTermsValidation.Error(BrandErrorCodes.UnknownFeatureMessage, BrandErrorCodes.UnknownFeature, FeatureCodeField);
        }

        if (command.Limit is not null && !feature.IsCounted)
        {
            return SubscriptionTermsValidation.Error(BrandErrorCodes.LimitOnUncountedFeatureMessage, BrandErrorCodes.LimitOnUncountedFeature, LimitField);
        }

        if (command.Limit < 0)
        {
            return SubscriptionTermsValidation.Error(BrandErrorCodes.NegativeLimitMessage, BrandErrorCodes.NegativeLimit, LimitField);
        }

        var today = DateOnly.FromDateTime(now.UtcDateTime);
        if (SubscriptionTermsValidation.Validate(command.Price, command.Currency, command.ExpiredOn, today) is { } termsError)
        {
            return termsError;
        }

        if (await repository.FindCurrentSubscriptionAsync(command.BrandId, cancellationToken) is not { } subscription)
        {
            return Result.Conflict<BrandAddOnResponse>(BrandErrorCodes.NoSubscriptionMessage, BrandErrorCodes.NoSubscription);
        }

        var addOns = await repository.ListAddOnsIncludingRemovedAsync(subscription.Id, cancellationToken);
        foreach (var replaced in addOns.Where(addOn => !addOn.IsDeleted && addOn.FeatureCode == feature.Code))
        {
            replaced.Delete(now);
        }

        var given = SubscriptionFeature.Create(subscription.Id, feature.Code, command.Price, command.Currency!, command.Limit, command.ExpiredOn, now);
        repository.Add(given);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return BrandAddOnResponse.From(given);
    }
}

public sealed class RemoveBrandAddOnUseCase(IBrandSubscriptionRepository repository, IUnitOfWork unitOfWork, TimeProvider timeProvider)
    : IUseCase<RemoveBrandAddOnCommand, BrandAddOnResponse>
{
    public async Task<Result<BrandAddOnResponse>> ExecuteAsync(RemoveBrandAddOnCommand command, CancellationToken cancellationToken)
    {
        var subscription = await repository.FindCurrentSubscriptionAsync(command.BrandId, cancellationToken);
        var addOns = subscription is null ? [] : await repository.ListAddOnsIncludingRemovedAsync(subscription.Id, cancellationToken);
        var addOn = addOns.FirstOrDefault(candidate => candidate.Id == command.AddOnId);
        if (addOn is null || addOn.IsDeleted)
        {
            return Result.NotFound<BrandAddOnResponse>(BrandErrorCodes.AddOnNotFoundMessage, BrandErrorCodes.AddOnNotFound);
        }

        addOn.Delete(timeProvider.GetUtcNow());
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return BrandAddOnResponse.From(addOn);
    }
}

public sealed class RestoreBrandAddOnUseCase(IBrandSubscriptionRepository repository, IUnitOfWork unitOfWork)
    : IUseCase<RestoreBrandAddOnCommand, BrandAddOnResponse>
{
    public async Task<Result<BrandAddOnResponse>> ExecuteAsync(RestoreBrandAddOnCommand command, CancellationToken cancellationToken)
    {
        if (await repository.FindCurrentSubscriptionAsync(command.BrandId, cancellationToken) is not { } subscription)
        {
            return Result.NotFound<BrandAddOnResponse>(BrandErrorCodes.AddOnNotFoundMessage, BrandErrorCodes.AddOnNotFound);
        }

        var addOns = await repository.ListAddOnsIncludingRemovedAsync(subscription.Id, cancellationToken);
        var addOn = addOns.FirstOrDefault(candidate => candidate.Id == command.AddOnId);
        if (addOn is null || !addOn.IsDeleted)
        {
            return Result.NotFound<BrandAddOnResponse>(BrandErrorCodes.AddOnNotFoundMessage, BrandErrorCodes.AddOnNotFound);
        }

        if (addOns.Any(other => !other.IsDeleted && other.FeatureCode == addOn.FeatureCode))
        {
            return Result.Conflict<BrandAddOnResponse>(BrandErrorCodes.AddOnReplacedMessage, BrandErrorCodes.AddOnReplaced);
        }

        addOn.Restore();
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return BrandAddOnResponse.From(addOn);
    }
}
