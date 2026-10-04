using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Abstractions.Time;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Authorization;
using ClassManager.Core.Domain.Clients;

namespace ClassManager.Core.UseCases.Orders;

public sealed record ListDeliveryClassesQuery(Guid ClientId) : IQuery;

public sealed class ListDeliveryClassesUseCase(
    IClientRepository clientRepository,
    IStudentRepository studentRepository,
    IEnrollmentRepository enrollmentRepository,
    IClassGroupRepository classGroupRepository,
    IBusinessCalendarService businessCalendar,
    IAccessScopes accessScopes)
    : IUseCase<ListDeliveryClassesQuery, IReadOnlyList<DeliveryClassResponse>>
{
    private const string ClientNotFoundMessage = "The client does not exist.";

    public async Task<Result<IReadOnlyList<DeliveryClassResponse>>> ExecuteAsync(ListDeliveryClassesQuery command, CancellationToken cancellationToken)
    {
        var client = await clientRepository.GetByIdAsync(command.ClientId, cancellationToken);
        if (client is null
            || !await AccessRules.CanReachClientsAsync(accessScopes, clientRepository, [client.Id], Permissions.Orders.ViewAll, cancellationToken))
        {
            return Result.NotFound<IReadOnlyList<DeliveryClassResponse>>(ClientNotFoundMessage, ClientErrorCodes.NotFound);
        }

        var today = await businessCalendar.TodayAsync(cancellationToken);
        return Result.Success(
            await DeliveryClasses.ListAsync(client.Id, today, studentRepository, enrollmentRepository, classGroupRepository, cancellationToken));
    }
}
