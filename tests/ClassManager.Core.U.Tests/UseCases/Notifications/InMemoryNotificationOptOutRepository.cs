using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Domain.Notifications;

namespace ClassManager.Core.U.Tests.UseCases.Notifications;

internal sealed class InMemoryNotificationOptOutRepository : INotificationOptOutRepository
{
    private readonly List<NotificationOptOut> _optOuts = [];

    public IReadOnlyList<NotificationOptOut> OptOuts => _optOuts;

    public Task<IReadOnlyList<NotificationOptOut>> ListAsync(Guid accountId, CancellationToken cancellationToken) =>
        Task.FromResult<IReadOnlyList<NotificationOptOut>>([.. _optOuts.Where(optOut => optOut.AccountId == accountId)]);

    public Task<NotificationOptOut?> FindAsync(
        Guid accountId, NotificationCategory category, NotificationChannel channel, CancellationToken cancellationToken) =>
        Task.FromResult(_optOuts.FirstOrDefault(optOut => optOut.AccountId == accountId && optOut.Category == category && optOut.Channel == channel));

    public Task<IReadOnlySet<Guid>> ListOptedOutAsync(
        IReadOnlyCollection<Guid> accountIds, NotificationCategory category, NotificationChannel channel, CancellationToken cancellationToken) =>
        Task.FromResult<IReadOnlySet<Guid>>(_optOuts
            .Where(optOut => accountIds.Contains(optOut.AccountId) && optOut.Category == category && optOut.Channel == channel)
            .Select(optOut => optOut.AccountId)
            .ToHashSet());

    public void Add(NotificationOptOut optOut) => _optOuts.Add(optOut);

    public void Remove(NotificationOptOut optOut) => _optOuts.Remove(optOut);

    public Task DeleteEveryOptOutOfAccountAsync(Guid accountId, CancellationToken cancellationToken)
    {
        _optOuts.RemoveAll(optOut => optOut.AccountId == accountId);
        return Task.CompletedTask;
    }
}
