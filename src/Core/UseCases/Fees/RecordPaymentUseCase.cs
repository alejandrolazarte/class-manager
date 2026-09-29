using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Abstractions.Time;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Clients;
using ClassManager.Core.Domain.Fees;

namespace ClassManager.Core.UseCases.Fees;

public sealed record RecordPaymentRequest(decimal? Amount, string? Month, DateOnly? PaidOn, PaymentMethod? Method, string? Notes)
{
    public RecordPaymentCommand ToCommand(Guid clientId) => new(clientId, Amount, Month, PaidOn, Method, Notes);
}

public sealed record RecordPaymentCommand(Guid ClientId, decimal? Amount, string? Month, DateOnly? PaidOn, PaymentMethod? Method, string? Notes);

public sealed class RecordPaymentUseCase(
    IClientRepository clientRepository,
    IPaymentRepository paymentRepository,
    IUnitOfWork unitOfWork,
    IBusinessCalendarService businessCalendar,
    TimeProvider timeProvider,
    IAccessScopes accessScopes,
    ICurrentMember currentMember)
    : IUseCase<RecordPaymentCommand, PaymentResponse>
{
    private const string ClientNotFoundMessage = "The client does not exist.";

    public async Task<Result<PaymentResponse>> ExecuteAsync(RecordPaymentCommand command, CancellationToken cancellationToken)
    {
        var client = await clientRepository.GetByIdAsync(command.ClientId, cancellationToken);
        if (client is null)
        {
            return Result.NotFound<PaymentResponse>(ClientNotFoundMessage, ClientErrorCodes.NotFound);
        }

        if (!await MoneyRules.CanCollectFromAsync(accessScopes, clientRepository, client.Id, cancellationToken))
        {
            return AccessRules.NotYours();
        }

        var today = await businessCalendar.TodayAsync(cancellationToken);
        var month = command.Month is null
            ? BillingMonth.From(today)
            : BillingMonth.Parse(command.Month, nameof(RecordPaymentCommand.Month));
        if (month.IsFailure)
        {
            return month.Error!;
        }

        var access = await currentMember.GetAccessAsync(cancellationToken);
        var payment = Payment.Create(
            client.Id,
            command.Amount,
            month.Value!,
            command.PaidOn ?? today,
            command.Method,
            command.Notes,
            today,
            timeProvider.GetUtcNow(),
            access?.UserId);
        if (payment.IsFailure)
        {
            return payment.Error!;
        }

        paymentRepository.Add(payment.Value!);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return PaymentResponse.From(payment.Value!);
    }
}
