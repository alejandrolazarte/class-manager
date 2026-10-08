using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Fees;

namespace ClassManager.Core.UseCases.Fees;

public sealed record RestorePaymentCommand(Guid PaymentId) : ICommand;

public sealed class RestorePaymentUseCase(
    IPaymentRepository paymentRepository,
    IClientRepository clientRepository,
    IUnitOfWork unitOfWork,
    IAccessScopes accessScopes,
    ICurrentMember currentMember)
    : IUseCase<RestorePaymentCommand, PaymentResponse>
{
    private const string NotFoundMessage = "The deleted payment does not exist.";

    public async Task<Result<PaymentResponse>> ExecuteAsync(RestorePaymentCommand command, CancellationToken cancellationToken)
    {
        var payment = await paymentRepository.FindDeletedForUpdateAsync(command.PaymentId, cancellationToken);
        if (payment is null)
        {
            return Result.NotFound<PaymentResponse>(NotFoundMessage, FeeErrorCodes.PaymentNotFound);
        }

        if (!await MoneyRules.CanUndoAsync(
            accessScopes, clientRepository, currentMember, payment.ClientId, payment.RecordedByUserId, cancellationToken))
        {
            return AccessRules.NotYours();
        }

        payment.Restore();
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return PaymentResponse.From(payment);
    }
}
