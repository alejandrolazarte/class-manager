namespace ClassManager.Api.Orders;

internal interface IExpiredOrderCancellationService
{
    Task<int> CancelExpiredOrdersAsync(CancellationToken cancellationToken);
}
