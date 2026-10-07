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
    IClientAccountRepository clientAccountRepository,
    IClientInvitationRepository invitationRepository,
    IIdentityService identityService,
    IBusinessCalendarService businessCalendar,
    IAccessScopes accessScopes,
    TimeProvider timeProvider)
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
        var accounts = await clientAccountRepository.ListByClientAsync(client.Id, cancellationToken);
        var pendingInvitations = await invitationRepository.ListPendingByClientAsync(client.Id, timeProvider.GetUtcNow(), cancellationToken);
        var clientAppAccess = await AppAccessOfAsync(null, accounts, pendingInvitations, cancellationToken);
        var studentAppAccesses = new Dictionary<Guid, StudentAppAccessResponse>();
        foreach (var student in students)
        {
            studentAppAccesses[student.Id] = await AppAccessOfAsync(student.Id, accounts, pendingInvitations, cancellationToken);
        }

        return ClientDetailsResponse.From(client, students, billingPlanChanges, today, clientAppAccess, studentAppAccesses);
    }

    private async Task<StudentAppAccessResponse> AppAccessOfAsync(
        Guid? studentId,
        IReadOnlyList<ClientAccount> accounts,
        IReadOnlyList<ClientInvitation> pendingInvitations,
        CancellationToken cancellationToken)
    {
        var accountUserIds = accounts.Where(account => account.StudentId == studentId).Select(account => account.UserId).ToList();
        var signInEmail = await SignInEmails.FirstAsync(identityService, accountUserIds, cancellationToken);
        var pendingInvitation = pendingInvitations.FirstOrDefault(invitation => invitation.StudentId == studentId);
        return StudentAppAccessResponse.From(accountUserIds.Count > 0, signInEmail, pendingInvitation);
    }
}
