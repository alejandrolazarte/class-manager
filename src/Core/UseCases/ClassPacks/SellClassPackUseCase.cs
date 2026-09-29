using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Abstractions.Time;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.ClassPacks;
using ClassManager.Core.Domain.Clients;
using ClassManager.Core.Domain.Fees;
using ClassManager.Core.UseCases.Fees;

namespace ClassManager.Core.UseCases.ClassPacks;

public sealed record SellClassPackRequest(
    Guid? ClassPackId,
    decimal? Price,
    DateOnly? PurchasedOn,
    PaymentMethod? Method,
    string? Notes,
    Guid? TrialLessonId = null)
{
    public SellClassPackCommand ToCommand(Guid clientId) => new(clientId, ClassPackId, Price, PurchasedOn, Method, Notes, TrialLessonId);
}

public sealed record SellClassPackCommand(
    Guid ClientId,
    Guid? ClassPackId,
    decimal? Price,
    DateOnly? PurchasedOn,
    PaymentMethod? Method,
    string? Notes,
    Guid? TrialLessonId = null);

public sealed class SellClassPackUseCase(
    IClientRepository clientRepository,
    IClassPackRepository classPackRepository,
    IClassPackPurchaseRepository purchaseRepository,
    IPrivateLessonRepository privateLessonRepository,
    IUnitOfWork unitOfWork,
    IBusinessCalendarService businessCalendar,
    TimeProvider timeProvider,
    IAccessScopes accessScopes,
    ICurrentMember currentMember)
    : IUseCase<SellClassPackCommand, ClassPackPurchaseResponse>
{
    private const string ClientNotFoundMessage = "The client does not exist.";
    private const string TrialNotDeductibleMessage = "That trial class can't be deducted from this sale.";

    public async Task<Result<ClassPackPurchaseResponse>> ExecuteAsync(SellClassPackCommand command, CancellationToken cancellationToken)
    {
        var client = await clientRepository.GetByIdAsync(command.ClientId, cancellationToken);
        if (client is null)
        {
            return Result.NotFound<ClassPackPurchaseResponse>(ClientNotFoundMessage, ClientErrorCodes.NotFound);
        }

        if (!await MoneyRules.CanCollectFromAsync(accessScopes, clientRepository, client.Id, cancellationToken))
        {
            return AccessRules.NotYours();
        }

        if (command.ClassPackId is null)
        {
            return ClassPackFailures.PackRequired(nameof(SellClassPackCommand.ClassPackId));
        }

        var classPack = await classPackRepository.GetByIdAsync(command.ClassPackId.Value, cancellationToken);
        if (classPack is null)
        {
            return ClassPackFailures.NotFound();
        }

        var today = await businessCalendar.TodayAsync(cancellationToken);
        var access = await currentMember.GetAccessAsync(cancellationToken);
        var purchase = ClassPackPurchase.Sell(
            client.Id,
            classPack,
            command.Price,
            command.PurchasedOn ?? today,
            command.Method,
            command.Notes,
            today,
            timeProvider.GetUtcNow(),
            access?.UserId);
        if (purchase.IsFailure)
        {
            return purchase.Error!;
        }

        if (command.TrialLessonId is not null)
        {
            if (!await IsDeductibleTrialAsync(client.Id, command.TrialLessonId.Value, cancellationToken))
            {
                return Result.Validation<ClassPackPurchaseResponse>(
                    TrialNotDeductibleMessage, ClassPackErrorCodes.TrialNotDeductible, nameof(SellClassPackCommand.TrialLessonId));
            }

            purchase.Value!.DeductTrial(command.TrialLessonId.Value);
        }

        purchaseRepository.Add(purchase.Value!);
        try
        {
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (UniqueConstraintViolationException)
        {
            return Result.Validation<ClassPackPurchaseResponse>(
                TrialNotDeductibleMessage, ClassPackErrorCodes.TrialNotDeductible, nameof(SellClassPackCommand.TrialLessonId));
        }

        return ClassPackPurchaseResponse.From(purchase.Value!);
    }

    private async Task<bool> IsDeductibleTrialAsync(Guid clientId, Guid trialLessonId, CancellationToken cancellationToken)
    {
        var paidTrials = await privateLessonRepository.ListPaidTrialsByClientAsync(clientId, cancellationToken);
        return paidTrials.Any(trial => trial.PrivateLessonId == trialLessonId)
            && !await purchaseRepository.IsTrialDeductedAsync(trialLessonId, cancellationToken);
    }
}
