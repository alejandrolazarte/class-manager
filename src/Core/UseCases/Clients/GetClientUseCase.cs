using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Abstractions.Time;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Clients;

namespace ClassManager.Core.UseCases.Clients;

public sealed record GetClientQuery(Guid ClientId) : IQuery;

public sealed class GetClientUseCase(
    IClientRepository clientRepository,
    IStudentRepository studentRepository,
    IFeeScheduleRepository feeScheduleRepository,
    IBusinessCalendarService businessCalendar,
    IAccessScopes accessScopes)
    : IUseCase<GetClientQuery, ClientDetailsResponse>
{
    private const string NotFoundMessage = "The client does not exist.";

    public async Task<Result<ClientDetailsResponse>> ExecuteAsync(GetClientQuery command, CancellationToken cancellationToken)
    {
        var client = await clientRepository.GetByIdAsync(command.ClientId, cancellationToken);
        if (client is null || !await AccessRules.CanReachClientsAsync(accessScopes, clientRepository, [client.Id], cancellationToken))
        {
            return Result.NotFound<ClientDetailsResponse>(NotFoundMessage, ClientErrorCodes.NotFound);
        }

        var students = await studentRepository.ListByClientAsync(client.Id, cancellationToken);
        var billingPlanChanges = await feeScheduleRepository.ListClientPlanChangesAsync([client.Id], cancellationToken);
        var today = await businessCalendar.TodayAsync(cancellationToken);

        return ClientDetailsResponse.From(client, students, billingPlanChanges, today);
    }
}
