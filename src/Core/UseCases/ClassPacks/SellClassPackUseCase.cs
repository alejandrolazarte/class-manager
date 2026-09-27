using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Time;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.ClassPacks;
using ClassManager.Core.Domain.Clients;
using ClassManager.Core.Domain.Fees;

namespace ClassManager.Core.UseCases.ClassPacks;

public sealed record SellClassPackRequest(Guid? ClassPackId, decimal? Price, DateOnly? PurchasedOn, PaymentMethod? Method, string? Notes)
{
    public SellClassPackCommand ToCommand(Guid clientId) => new(clientId, ClassPackId, Price, PurchasedOn, Method, Notes);
}

public sealed record SellClassPackCommand(Guid ClientId, Guid? ClassPackId, decimal? Price, DateOnly? PurchasedOn, PaymentMethod? Method, string? Notes);

public sealed class SellClassPackUseCase(
    IClientRepository clientRepository,
    IClassPackRepository classPackRepository,
    IClassPackPurchaseRepository purchaseRepository,
    IUnitOfWork unitOfWork,
    IBusinessCalendarService businessCalendar,
    TimeProvider timeProvider)
    : IUseCase<SellClassPackCommand, ClassPackPurchaseResponse>
{
    private const string ClientNotFoundMessage = "The client does not exist.";

    public async Task<Result<ClassPackPurchaseResponse>> ExecuteAsync(SellClassPackCommand command, CancellationToken cancellationToken)
    {
        var client = await clientRepository.GetByIdAsync(command.ClientId, cancellationToken);
        if (client is null)
        {
            return Result.NotFound<ClassPackPurchaseResponse>(ClientNotFoundMessage, ClientErrorCodes.NotFound);
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
        var purchase = ClassPackPurchase.Sell(
            client.Id, classPack, command.Price, command.PurchasedOn ?? today, command.Method, command.Notes, today, timeProvider.GetUtcNow());
        if (purchase.IsFailure)
        {
            return purchase.Error!;
        }

        purchaseRepository.Add(purchase.Value!);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ClassPackPurchaseResponse.From(purchase.Value!);
    }
}
