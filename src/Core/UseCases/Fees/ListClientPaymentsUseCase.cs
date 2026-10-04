using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Clients;

namespace ClassManager.Core.UseCases.Fees;

public sealed record ListClientPaymentsQuery(Guid ClientId) : IQuery;

public sealed class ListClientPaymentsUseCase(
    IClientRepository clientRepository,
    IPaymentRepository paymentRepository,
    IIdentityService identityService,
    IAccessScopes accessScopes)
    : IUseCase<ListClientPaymentsQuery, IReadOnlyList<PaymentResponse>>
{
    public const int PaymentLimit = 24;

    private const string ClientNotFoundMessage = "The client does not exist.";

    public async Task<Result<IReadOnlyList<PaymentResponse>>> ExecuteAsync(ListClientPaymentsQuery command, CancellationToken cancellationToken)
    {
        var client = await clientRepository.GetByIdAsync(command.ClientId, cancellationToken);
        if (client is null || !await MoneyRules.CanCollectFromAsync(accessScopes, clientRepository, client.Id, cancellationToken))
        {
            return Result.NotFound<IReadOnlyList<PaymentResponse>>(ClientNotFoundMessage, ClientErrorCodes.NotFound);
        }

        var payments = await paymentRepository.ListByClientAsync(client.Id, PaymentLimit, cancellationToken);
        var recorderNames = (await identityService.ListAccountsAsync(
                [.. payments.Select(payment => payment.RecordedByUserId).OfType<Guid>().Distinct()], cancellationToken))
            .ToDictionary(account => account.UserId, account => account.FullName);

        return Result.Success<IReadOnlyList<PaymentResponse>>(
        [
            .. payments.Select(payment => PaymentResponse.From(
                payment,
                payment.RecordedByUserId is { } userId ? recorderNames.GetValueOrDefault(userId) : null)),
        ]);
    }
}
