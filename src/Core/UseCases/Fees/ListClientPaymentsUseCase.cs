using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Clients;

namespace ClassManager.Core.UseCases.Fees;

public sealed record ListClientPaymentsQuery(Guid ClientId);

public sealed class ListClientPaymentsUseCase(IClientRepository clientRepository, IPaymentRepository paymentRepository)
    : IUseCase<ListClientPaymentsQuery, IReadOnlyList<PaymentResponse>>
{
    public const int PaymentLimit = 24;

    private const string ClientNotFoundMessage = "The client does not exist.";

    public async Task<Result<IReadOnlyList<PaymentResponse>>> ExecuteAsync(ListClientPaymentsQuery command, CancellationToken cancellationToken)
    {
        var client = await clientRepository.GetByIdAsync(command.ClientId, cancellationToken);
        if (client is null)
        {
            return Result.NotFound<IReadOnlyList<PaymentResponse>>(ClientNotFoundMessage, ClientErrorCodes.NotFound);
        }

        var payments = await paymentRepository.ListByClientAsync(client.Id, PaymentLimit, cancellationToken);

        return Result.Success<IReadOnlyList<PaymentResponse>>([.. payments.Select(PaymentResponse.From)]);
    }
}
