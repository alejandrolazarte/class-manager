using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Domain.Orders;

namespace ClassManager.Core.UseCases.Families;

internal static class FamilyOrderResponses
{
    public static async Task<IReadOnlyList<FamilyOrderResponse>> OfAsync(
        IReadOnlyList<Order> orders,
        IClassGroupRepository classGroupRepository,
        CancellationToken cancellationToken)
    {
        var classGroupNames = orders.Any(order => order.DeliveryClassGroupId is not null)
            ? (await classGroupRepository.ListAllAsync(cancellationToken)).ToDictionary(classGroup => classGroup.Id, classGroup => classGroup.Name)
            : [];
        return
        [
            .. orders.Select(order => FamilyOrderResponse.From(
                order, order.DeliveryClassGroupId is { } classGroupId ? classGroupNames.GetValueOrDefault(classGroupId) : null)),
        ];
    }
}
