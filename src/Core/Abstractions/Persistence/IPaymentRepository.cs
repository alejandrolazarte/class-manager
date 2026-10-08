using ClassManager.Core.Domain.Fees;

namespace ClassManager.Core.Abstractions.Persistence;

public interface IPaymentRepository
{
    void Add(Payment payment);

    void Remove(Payment payment);

    Task<Payment?> GetForUpdateAsync(Guid paymentId, CancellationToken cancellationToken);

    Task<IReadOnlyList<Payment>> ListByClientAsync(Guid clientId, int limit, CancellationToken cancellationToken);

    Task<IReadOnlyDictionary<Guid, decimal>> SumByClientForMonthAsync(DateOnly monthFirstDay, CancellationToken cancellationToken);

    Task<IReadOnlyList<ClientPaidMonth>> ListPaidMonthsByClientsAsync(IReadOnlyCollection<Guid> clientIds, CancellationToken cancellationToken);
}
