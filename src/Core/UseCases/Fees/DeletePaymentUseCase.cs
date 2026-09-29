using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Fees;

namespace ClassManager.Core.UseCases.Fees;

public sealed record DeletePaymentCommand(Guid PaymentId);

public sealed class DeletePaymentUseCase(
    IPaymentRepository paymentRepository,
    IClientRepository clientRepository,
    IUnitOfWork unitOfWork,
    IAccessScopes accessScopes,
    ICurrentMember currentMember)
    : IUseCase<DeletePaymentCommand, PaymentResponse>
{
    private const string NotFoundMessage = "The payment does not exist.";

    public async Task<Result<PaymentResponse>> ExecuteAsync(DeletePaymentCommand command, CancellationToken cancellationToken)
    {
        var payment = await paymentRepository.GetForUpdateAsync(command.PaymentId, cancellationToken);
        if (payment is null)
        {
            return Result.NotFound<PaymentResponse>(NotFoundMessage, FeeErrorCodes.PaymentNotFound);
        }

        if (!await MoneyRules.CanUndoAsync(
            accessScopes, clientRepository, currentMember, payment.ClientId, payment.RecordedByUserId, cancellationToken))
        {
            return AccessRules.NotYours();
        }

        paymentRepository.Remove(payment);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return PaymentResponse.From(payment);
    }
}
