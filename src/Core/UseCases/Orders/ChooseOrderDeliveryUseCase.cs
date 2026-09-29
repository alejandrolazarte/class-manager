using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Abstractions.Time;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Orders;

namespace ClassManager.Core.UseCases.Orders;

public sealed record ChooseOrderDeliveryRequest(DeliveryMethod? Delivery, Guid? ClassGroupId)
{
    public ChooseOrderDeliveryCommand ToCommand(Guid orderId) => new(orderId, Delivery, ClassGroupId);
}

public sealed record ChooseOrderDeliveryCommand(Guid OrderId, DeliveryMethod? Delivery, Guid? ClassGroupId);

public sealed class ChooseOrderDeliveryUseCase(
    IOrderRepository orderRepository,
    IClientRepository clientRepository,
    IClassGroupRepository classGroupRepository,
    IStudentRepository studentRepository,
    IEnrollmentRepository enrollmentRepository,
    IUnitOfWork unitOfWork,
    IBusinessCalendarService businessCalendar,
    IAccessScopes accessScopes)
    : IUseCase<ChooseOrderDeliveryCommand, OrderResponse>
{
    public async Task<Result<OrderResponse>> ExecuteAsync(ChooseOrderDeliveryCommand command, CancellationToken cancellationToken)
    {
        var order = await orderRepository.GetForUpdateAsync(command.OrderId, cancellationToken);
        if (order is null || !await OrderAccess.CanReachAsync(accessScopes, clientRepository, order, cancellationToken))
        {
            return OrderFailures.NotFound();
        }

        var today = await businessCalendar.TodayAsync(cancellationToken);
        var delivery = await DeliveryClasses.ChooseAsync(
            order, command.Delivery, command.ClassGroupId, today, studentRepository, enrollmentRepository, classGroupRepository, cancellationToken);
        if (delivery.IsFailure)
        {
            return delivery.Error!;
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return await OrderResponses.OfAsync(order, clientRepository, classGroupRepository, cancellationToken);
    }
}
