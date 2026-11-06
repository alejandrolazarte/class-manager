using ClassManager.Backoffice.Core.Brands;
using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Audit;
using ClassManager.Core.Common;
using ClassManager.Core.UseCases;
using ClassManager.Subscriptions.Catalog;
using ClassManager.Subscriptions.Subscribers;

namespace ClassManager.Backoffice.Core.UseCases;

public sealed record ChangePlanSalesFeeCommand([property: AuditPlain] string PlanCode, decimal Percent) : ICommand;

public sealed record GetBrandSalesFeeQuery(long BrandId) : IQuery;

public sealed record GiveBrandSalesFeeCommand(long BrandId, decimal Percent, [property: AuditPersonal] string? Note) : ICommand;

public sealed record RemoveBrandSalesFeeCommand(long BrandId) : ICommand;

public sealed record BrandSalesFeeResponse(decimal Percent, bool IsBrandFee, decimal PlanPercent, string? Note);

public sealed class ChangePlanSalesFeeUseCase(IBrandSubscriptionRepository repository, IUnitOfWork unitOfWork)
    : IUseCase<ChangePlanSalesFeeCommand, CatalogPlanResponse>
{
    private const string PercentField = "Percent";

    public async Task<Result<CatalogPlanResponse>> ExecuteAsync(ChangePlanSalesFeeCommand command, CancellationToken cancellationToken)
    {
        if (await repository.FindActivePlanForUpdateAsync(command.PlanCode, cancellationToken) is not { } plan)
        {
            return Result.NotFound<CatalogPlanResponse>(BrandErrorCodes.UnknownPlanMessage, BrandErrorCodes.UnknownPlan);
        }

        if (!CatalogRules.IsPercent(command.Percent))
        {
            return SubscriptionTermsValidation.Error(BrandErrorCodes.SalesFeeNotPercentMessage, BrandErrorCodes.SalesFeeNotPercent, PercentField);
        }

        plan.ChangeSalesFee(command.Percent);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return CatalogPlanResponse.From(plan);
    }
}

public sealed class GetBrandSalesFeeUseCase(IBrandSubscriptionRepository repository)
    : IUseCase<GetBrandSalesFeeQuery, BrandSalesFeeResponse>
{
    public async Task<Result<BrandSalesFeeResponse>> ExecuteAsync(GetBrandSalesFeeQuery command, CancellationToken cancellationToken)
    {
        if (!await repository.BrandExistsAsync(command.BrandId, cancellationToken))
        {
            return Result.NotFound<BrandSalesFeeResponse>(BrandErrorCodes.BrandNotFoundMessage, BrandErrorCodes.BrandNotFound);
        }

        return await BrandSalesFees.ResponseAsync(repository, command.BrandId, cancellationToken);
    }
}

public sealed class GiveBrandSalesFeeUseCase(IBrandSubscriptionRepository repository, IUnitOfWork unitOfWork, TimeProvider timeProvider)
    : IUseCase<GiveBrandSalesFeeCommand, BrandSalesFeeResponse>
{
    private const string PercentField = "Percent";
    private const string NoteField = "Note";

    public async Task<Result<BrandSalesFeeResponse>> ExecuteAsync(GiveBrandSalesFeeCommand command, CancellationToken cancellationToken)
    {
        if (!await repository.BrandExistsAsync(command.BrandId, cancellationToken))
        {
            return Result.NotFound<BrandSalesFeeResponse>(BrandErrorCodes.BrandNotFoundMessage, BrandErrorCodes.BrandNotFound);
        }

        if (!CatalogRules.IsPercent(command.Percent))
        {
            return SubscriptionTermsValidation.Error(BrandErrorCodes.SalesFeeNotPercentMessage, BrandErrorCodes.SalesFeeNotPercent, PercentField);
        }

        if (command.Note?.Trim() is { Length: > CatalogRules.NoteMaxLength })
        {
            return SubscriptionTermsValidation.Error(BrandErrorCodes.NoteTooLongMessage, BrandErrorCodes.NoteTooLong, NoteField);
        }

        var now = timeProvider.GetUtcNow();
        var current = await repository.FindCurrentSalesFeeAsync(command.BrandId, cancellationToken);
        current?.Delete(now);
        repository.Add(SalesFeeOverride.Create(command.BrandId, command.Percent, command.Note, now));
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return await BrandSalesFees.ResponseAsync(repository, command.BrandId, cancellationToken);
    }
}

public sealed class RemoveBrandSalesFeeUseCase(IBrandSubscriptionRepository repository, IUnitOfWork unitOfWork, TimeProvider timeProvider)
    : IUseCase<RemoveBrandSalesFeeCommand, BrandSalesFeeResponse>
{
    public async Task<Result<BrandSalesFeeResponse>> ExecuteAsync(RemoveBrandSalesFeeCommand command, CancellationToken cancellationToken)
    {
        if (await repository.FindCurrentSalesFeeAsync(command.BrandId, cancellationToken) is not { } salesFee)
        {
            return Result.NotFound<BrandSalesFeeResponse>(BrandErrorCodes.SalesFeeNotFoundMessage, BrandErrorCodes.SalesFeeNotFound);
        }

        salesFee.Delete(timeProvider.GetUtcNow());
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return await BrandSalesFees.ResponseAsync(repository, command.BrandId, cancellationToken);
    }
}

internal static class BrandSalesFees
{
    public static async Task<BrandSalesFeeResponse> ResponseAsync(IBrandSubscriptionRepository repository, long brandId, CancellationToken cancellationToken)
    {
        var subscription = await repository.FindCurrentSubscriptionAsync(brandId, cancellationToken);
        var plans = await repository.ListActivePlansAsync(cancellationToken);
        var plan = plans.FirstOrDefault(candidate => candidate.Code == subscription?.PlanCode);
        var brandFee = await repository.FindCurrentSalesFeeAsync(brandId, cancellationToken);
        return new BrandSalesFeeResponse(
            SalesFees.PercentOf(plan, brandFee),
            brandFee is not null,
            plan?.SalesFeePercent ?? 0m,
            brandFee?.Note);
    }
}
