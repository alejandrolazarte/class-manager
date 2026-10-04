using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Orders;

namespace ClassManager.Core.UseCases.Orders;

public sealed record DeliveryClassResponse(Guid ClassGroupId, string ClassGroupName, string StudentFullName);

internal static class DeliveryClasses
{
    private const string ClassNotValidMessage = "Choose a class one of the client's students attends.";

    public static async Task<IReadOnlyList<DeliveryClassResponse>> ListAsync(
        Guid clientId,
        DateOnly today,
        IStudentRepository studentRepository,
        IEnrollmentRepository enrollmentRepository,
        IClassGroupRepository classGroupRepository,
        CancellationToken cancellationToken)
    {
        var classGroupNames = (await classGroupRepository.ListActiveAsync(cancellationToken))
            .ToDictionary(classGroup => classGroup.Id, classGroup => classGroup.Name);
        var deliveryClasses = new List<DeliveryClassResponse>();
        foreach (var student in await studentRepository.ListByClientAsync(clientId, cancellationToken))
        {
            foreach (var enrollment in await enrollmentRepository.ListCurrentByStudentAsync(student.Id, today, cancellationToken))
            {
                if (classGroupNames.TryGetValue(enrollment.ClassGroupId, out var classGroupName))
                {
                    deliveryClasses.Add(new DeliveryClassResponse(enrollment.ClassGroupId, classGroupName, student.FullName));
                }
            }
        }

        return deliveryClasses;
    }

    public static async Task<Result> ChooseAsync(
        Order order,
        DeliveryMethod? delivery,
        Guid? classGroupId,
        DateOnly today,
        IStudentRepository studentRepository,
        IEnrollmentRepository enrollmentRepository,
        IClassGroupRepository classGroupRepository,
        CancellationToken cancellationToken)
    {
        if (delivery == DeliveryMethod.InClass && classGroupId is { } chosenClassGroupId && order.ClientId is { } clientId)
        {
            var deliveryClasses = await ListAsync(clientId, today, studentRepository, enrollmentRepository, classGroupRepository, cancellationToken);
            if (deliveryClasses.All(deliveryClass => deliveryClass.ClassGroupId != chosenClassGroupId))
            {
                return Result.Validation(ClassNotValidMessage, OrderErrorCodes.ClassNotValid, nameof(Order.DeliveryClassGroupId));
            }
        }

        return order.ChooseDelivery(delivery, classGroupId);
    }
}
