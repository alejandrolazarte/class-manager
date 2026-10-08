namespace ClassManager.Infrastructure.Persistence.Repositories;

internal sealed class PaymentRepository(AppDbContext context) : IPaymentRepository
{
    public void Add(Payment payment) => context.Payments.Add(payment);

    public void Remove(Payment payment) => context.Payments.Remove(payment);

    public Task<Payment?> GetForUpdateAsync(Guid paymentId, CancellationToken cancellationToken) =>
        context.Payments.FirstOrDefaultAsync(payment => payment.Id == paymentId, cancellationToken);

    public Task<Payment?> FindDeletedForUpdateAsync(Guid paymentId, CancellationToken cancellationToken) =>
        context.Payments
            .IgnoreQueryFilters([SoftDeleteModelBuilderExtensions.SoftDeleteQueryFilter])
            .FirstOrDefaultAsync(payment => payment.Id == paymentId && payment.IsDeleted, cancellationToken);

    public async Task<IReadOnlyList<Payment>> ListByClientAsync(Guid clientId, int limit, CancellationToken cancellationToken) =>
        await context.Payments.AsNoTracking()
            .Where(payment => payment.ClientId == clientId)
            .OrderByDescending(payment => payment.PaidOn)
            .ThenByDescending(payment => payment.CreatedAt)
            .Take(limit)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyDictionary<Guid, decimal>> SumByClientForMonthAsync(DateOnly monthFirstDay, CancellationToken cancellationToken) =>
        await context.Payments.AsNoTracking()
            .Where(payment => payment.Month == monthFirstDay)
            .GroupBy(payment => payment.ClientId)
            .Select(group => new { ClientId = group.Key, Paid = group.Sum(payment => payment.Amount) })
            .ToDictionaryAsync(group => group.ClientId, group => group.Paid, cancellationToken);

    public async Task<IReadOnlyList<ClientPaidMonth>> ListPaidMonthsByClientsAsync(IReadOnlyCollection<Guid> clientIds, CancellationToken cancellationToken) =>
        await context.Payments.AsNoTracking()
            .Where(payment => clientIds.Contains(payment.ClientId))
            .Select(payment => new { payment.ClientId, payment.Month })
            .Distinct()
            .Select(paidMonth => new ClientPaidMonth(paidMonth.ClientId, paidMonth.Month))
            .ToListAsync(cancellationToken);
}
