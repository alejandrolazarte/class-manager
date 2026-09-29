using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Common;

namespace ClassManager.Core.UseCases.Families;

public sealed record ListFamilyOrdersQuery;

public sealed class ListFamilyOrdersUseCase(IFamilyAccess familyAccess, IOrderRepository orderRepository)
    : IUseCase<ListFamilyOrdersQuery, IReadOnlyList<FamilyOrderResponse>>
{
    private const int OrderLimit = 50;

    public async Task<Result<IReadOnlyList<FamilyOrderResponse>>> ExecuteAsync(ListFamilyOrdersQuery command, CancellationToken cancellationToken)
    {
        var access = await familyAccess.GetAsync(cancellationToken);
        if (access is null)
        {
            return FamilyFailures.NoAccess();
        }

        var orders = await orderRepository.ListAsync(new OrderSearchCriteria(access.ClientId, false, OrderLimit), null, cancellationToken);
        return Result.Success<IReadOnlyList<FamilyOrderResponse>>([.. orders.Select(FamilyOrderResponse.From)]);
    }
}
