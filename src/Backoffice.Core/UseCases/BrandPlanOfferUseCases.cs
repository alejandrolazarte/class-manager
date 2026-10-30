using ClassManager.Backoffice.Core.Brands;
using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Audit;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Subscriptions;
using ClassManager.Core.UseCases;
using ClassManager.Records;
using ClassManager.Subscriptions.Catalog;
using ClassManager.Subscriptions.Offers;

namespace ClassManager.Backoffice.Core.UseCases;

public sealed record OfferBrandPlanCommand(
    long BrandId,
    [property: AuditPlain] string? PlanCode,
    decimal Price,
    [property: AuditPersonal] string? Note,
    DateOnly? ExpiredOn) : ICommand;

public sealed record WithdrawBrandPlanOfferCommand(long BrandId) : ICommand;

public sealed record BrandPlanOfferResponse(
    string PlanCode,
    decimal Price,
    string Currency,
    string? Note,
    DateTimeOffset CreatedOn,
    DateOnly? ExpiredOn,
    bool IsActive,
    string PaymentLink)
{
    public static BrandPlanOfferResponse From(PlanOffer offer, DateOnly today, IBrandBillingLinks billingLinks) =>
        new(offer.PlanCode, offer.Price, offer.Currency, offer.Note, offer.CreatedOn, offer.ExpiredOn, offer.IsActiveOn(today), billingLinks.PlanOfferPayment());
}

public sealed class OfferBrandPlanUseCase(
    IBrandSubscriptionRepository repository,
    IBrandBillingLinks billingLinks,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider)
    : IUseCase<OfferBrandPlanCommand, BrandPlanOfferResponse>
{
    private const string PlanCodeField = "PlanCode";
    private const string PriceField = "Price";
    private const string NoteField = "Note";
    private const string ExpiredOnField = "ExpiredOn";

    public async Task<Result<BrandPlanOfferResponse>> ExecuteAsync(OfferBrandPlanCommand command, CancellationToken cancellationToken)
    {
        var now = timeProvider.GetUtcNow();
        var today = DateOnly.FromDateTime(now.UtcDateTime);
        if (!await repository.BrandExistsAsync(command.BrandId, cancellationToken))
        {
            return Result.NotFound<BrandPlanOfferResponse>(BrandErrorCodes.BrandNotFoundMessage, BrandErrorCodes.BrandNotFound);
        }

        var planCode = command.PlanCode?.Trim();
        var plans = await repository.ListActivePlansAsync(cancellationToken);
        if (planCode is null || !SelfServiceBilling.PlanCodes.Contains(planCode) || plans.All(plan => plan.Code != planCode))
        {
            return SubscriptionTermsValidation.Error(BrandErrorCodes.PlanNotOfferableMessage, BrandErrorCodes.PlanNotOfferable, PlanCodeField);
        }

        if (command.Price <= 0)
        {
            return SubscriptionTermsValidation.Error(BrandErrorCodes.OfferPriceNotPositiveMessage, BrandErrorCodes.OfferPriceNotPositive, PriceField);
        }

        if (command.ExpiredOn < today)
        {
            return SubscriptionTermsValidation.Error(BrandErrorCodes.EndDateInThePastMessage, BrandErrorCodes.EndDateInThePast, ExpiredOnField);
        }

        if (command.Note?.Trim() is { Length: > CatalogRules.NoteMaxLength })
        {
            return SubscriptionTermsValidation.Error(BrandErrorCodes.NoteTooLongMessage, BrandErrorCodes.NoteTooLong, NoteField);
        }

        var subscription = await repository.FindCurrentSubscriptionAsync(command.BrandId, cancellationToken);
        if (subscription is { ExternalSubscriptionId: not null } && subscription.IsActiveOn(today))
        {
            return Result.Conflict<BrandPlanOfferResponse>(BrandErrorCodes.SubscriptionExternalMessage, BrandErrorCodes.SubscriptionExternal);
        }

        var countryCode = await repository.FindFirstBranchCountryCodeAsync(command.BrandId, cancellationToken);
        if (countryCode is null || !SelfServiceBilling.CountryCodes.Contains(countryCode))
        {
            return Result.Conflict<BrandPlanOfferResponse>(BrandErrorCodes.BillingNotAvailableMessage, BrandErrorCodes.BillingNotAvailable);
        }

        var current = await repository.FindCurrentOfferAsync(command.BrandId, cancellationToken);
        current?.Delete(now);
        var offer = PlanOffer.Create(command.BrandId, planCode, command.Price, SelfServiceBilling.Currency, command.Note, command.ExpiredOn, now);
        repository.Add(offer);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return BrandPlanOfferResponse.From(offer, today, billingLinks);
    }
}

public sealed class WithdrawBrandPlanOfferUseCase(
    IBrandSubscriptionRepository repository,
    IBrandBillingLinks billingLinks,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider)
    : IUseCase<WithdrawBrandPlanOfferCommand, BrandPlanOfferResponse>
{
    public async Task<Result<BrandPlanOfferResponse>> ExecuteAsync(WithdrawBrandPlanOfferCommand command, CancellationToken cancellationToken)
    {
        if (await repository.FindCurrentOfferAsync(command.BrandId, cancellationToken) is not { } offer)
        {
            return Result.NotFound<BrandPlanOfferResponse>(BrandErrorCodes.OfferNotFoundMessage, BrandErrorCodes.OfferNotFound);
        }

        var now = timeProvider.GetUtcNow();
        offer.Delete(now);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return BrandPlanOfferResponse.From(offer, DateOnly.FromDateTime(now.UtcDateTime), billingLinks);
    }
}
