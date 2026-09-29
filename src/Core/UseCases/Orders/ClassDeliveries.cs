using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Authorization;
using ClassManager.Core.Domain.ClassGroups;
using ClassManager.Core.Domain.Orders;

namespace ClassManager.Core.UseCases.Orders;

public sealed record ClassDeliveryLineResponse(string Name, int Quantity);

public sealed record ClassDeliveryResponse(Guid OrderId, string? ClientFullName, bool IsReady, IReadOnlyList<ClassDeliveryLineResponse> Lines);

internal static class ClassDeliveries
{
    private const string ClassGroupNotFoundMessage = "The class group does not exist.";

    public static async Task<Result<ClassGroup>> GetClassGroupInScopeAsync(
        Guid classGroupId,
        IClassGroupRepository classGroupRepository,
        IAccessScopes accessScopes,
        CancellationToken cancellationToken)
    {
        var classGroup = await classGroupRepository.GetByIdAsync(classGroupId, cancellationToken);
        if (classGroup is null)
        {
            return Result.NotFound<ClassGroup>(ClassGroupNotFoundMessage, ClassGroupErrorCodes.NotFound);
        }

        var scope = await accessScopes.ForInstructorsAsync(Permissions.Attendance.RecordAll, cancellationToken);
        return scope.Includes(classGroup.InstructorId) ? classGroup : AccessRules.NotYours();
    }

    public static ClassDeliveryResponse ResponseOf(Order order, string? clientFullName) =>
        new(
            order.Id,
            clientFullName,
            order.IsReady,
            [
                .. order.Lines
                    .Where(line => line.Kind == OrderLineKind.Product)
                    .Select(line => new ClassDeliveryLineResponse(line.Name, line.Quantity - line.RefundedQuantity)),
            ]);
}
