using ClassManager.Core.Domain.Fees;

namespace ClassManager.Core.Abstractions.Persistence;

public interface IPaymentRepository
{
    void Add(Payment payment);

    void Remove(Payment payment);

    Task<Payment?> GetForUpdateAsync(long paymentId, CancellationToken cancellationToken);

    Task<Payment?> FindDeletedForUpdateAsync(long paymentId, CancellationToken cancellationToken);

    Task<IReadOnlyList<Payment>> ListByClientAsync(long clientId, int limit, CancellationToken cancellationToken);

    Task<IReadOnlyDictionary<long, decimal>> SumByClientForMonthAsync(DateOnly monthFirstDay, string currencyCode, CancellationToken cancellationToken);

    Task<IReadOnlyList<ClientPaidMonth>> ListPaidMonthsByClientsAsync(IReadOnlyCollection<long> clientIds, CancellationToken cancellationToken);
}
