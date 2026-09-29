using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Domain.Orders;
using ClassManager.Core.UseCases.Orders;

namespace ClassManager.Api.Orders;

internal sealed partial class ExpiredOrderCancellationService(
    IServiceScopeFactory scopeFactory,
    TimeProvider timeProvider,
    ILogger<ExpiredOrderCancellationService> logger)
    : IExpiredOrderCancellationService
{
    public async Task<int> CancelExpiredOrdersAsync(CancellationToken cancellationToken)
    {
        IReadOnlyList<Guid> businessIds;
        await using (var directoryScope = scopeFactory.CreateAsyncScope())
        {
            var directory = directoryScope.ServiceProvider.GetRequiredService<IExpiredOrderDirectory>();
            businessIds = await directory.ListBusinessesWithRequestsCreatedBeforeAsync(
                timeProvider.GetUtcNow() - Order.RequestLifetime, cancellationToken);
        }

        var cancelledCount = 0;
        foreach (var businessId in businessIds)
        {
            await using var businessScope = scopeFactory.CreateAsyncScope();
            businessScope.ServiceProvider.GetRequiredService<ITenantScope>().Establish(businessId);
            var useCase = businessScope.ServiceProvider.GetRequiredService<IUseCase<CancelExpiredOrdersCommand, int>>();
            var result = await useCase.ExecuteAsync(new CancelExpiredOrdersCommand(), cancellationToken);
            if (result.IsSuccess)
            {
                cancelledCount += result.Value;
                LogCancelled(logger, result.Value, businessId);
            }
        }

        return cancelledCount;
    }

    [LoggerMessage(Level = LogLevel.Information, Message = "Cancelled {Count} unpaid orders of business {BusinessId}")]
    private static partial void LogCancelled(ILogger logger, int count, Guid businessId);
}
