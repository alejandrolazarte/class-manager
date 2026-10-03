using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Abstractions.Time;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.ClassPacks;
using ClassManager.Core.Domain.Clients;
using ClassManager.Core.Domain.Fees;
using ClassManager.Core.Domain.Orders;
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
    IOrderNumbers orderNumbers,
    IOrderRepository orderRepository,
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
        var now = timeProvider.GetUtcNow();
        var access = await currentMember.GetAccessAsync(cancellationToken);
        var purchase = ClassPackPurchase.Sell(
            client.Id,
            classPack,
            command.Price,
            command.PurchasedOn ?? today,
            command.Method,
            command.Notes,
            today,
            now,
            access?.UserId);
        if (purchase.IsFailure)
        {
            return purchase.Error!;
        }

        if (command.TrialLessonId is not null)
        {
            if (!await IsDeductibleTrialAsync(client.Id, command.TrialLessonId.Value, cancellationToken))
            {
                return TrialNotDeductible();
            }

            purchase.Value!.DeductTrial(command.TrialLessonId.Value);
        }

        var order = OrderOf(purchase.Value!, classPack, today);
        if (order.IsFailure)
        {
            return order.Error!;
        }

        try
        {
            await using var transaction = await unitOfWork.BeginTransactionAsync(cancellationToken);
            purchaseRepository.Add(purchase.Value!);
            order.Value!.AssignNumber(await orderNumbers.TakeNextAsync(cancellationToken));
            orderRepository.Add(order.Value);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch (UniqueConstraintViolationException)
        {
            return TrialNotDeductible();
        }

        return ClassPackPurchaseResponse.From(purchase.Value!);
    }

    private static Result<Order> OrderOf(ClassPackPurchase purchase, ClassPack classPack, DateOnly today)
    {
        var line = OrderLine.ForClassPack(classPack, purchase.Price);
        if (line.IsFailure)
        {
            return line.Error!;
        }

        line.Value!.LinkPurchase(purchase.Id);
        return Order.CounterSale(
            purchase.ClientId,
            [line.Value],
            purchase.Method,
            purchase.PurchasedOn,
            purchase.Notes,
            isDelivered: false,
            today,
            purchase.RecordedByUserId,
            purchase.CreatedAt);
    }

    private static Result<ClassPackPurchaseResponse> TrialNotDeductible() =>
        Result.Validation<ClassPackPurchaseResponse>(
            TrialNotDeductibleMessage, ClassPackErrorCodes.TrialNotDeductible, nameof(SellClassPackCommand.TrialLessonId));

    private async Task<bool> IsDeductibleTrialAsync(Guid clientId, Guid trialLessonId, CancellationToken cancellationToken)
    {
        var paidTrials = await privateLessonRepository.ListPaidTrialsByClientAsync(clientId, cancellationToken);
        return paidTrials.Any(trial => trial.PrivateLessonId == trialLessonId)
            && !await purchaseRepository.IsTrialDeductedAsync(trialLessonId, cancellationToken);
    }
}
