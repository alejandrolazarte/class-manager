using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Abstractions.Storage;
using ClassManager.Core.Abstractions.Time;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Clients;

using ClassManager.Core.UseCases.Fees;

namespace ClassManager.Core.UseCases.Clients;

public sealed record GetClientQuery(long ClientId) : IQuery;

public sealed class GetClientUseCase(
    IClientRepository clientRepository,
    IStudentRepository studentRepository,
    IStudentPlanRepository studentPlanRepository,
    IClassPackRepository classPackRepository,
    IClientAccountRepository clientAccountRepository,
    IClientInvitationRepository invitationRepository,
    IIdentityService identityService,
    IUserRepository userRepository,
    IBusinessCalendarService businessCalendar,
    IAccessScopes accessScopes,
    IUserPhotoUrlService userPhotoUrls,
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
        var today = await businessCalendar.TodayAsync(cancellationToken);
        var plans = FamilyPlansResponse.From(
            client.Id,
            students,
            await studentPlanRepository.ListByClientsAsync([client.Id], cancellationToken),
            await classPackRepository.ListAsync(includeInactive: true, cancellationToken),
            await classPackRepository.ListPriceChangesAsync(cancellationToken),
            today);
        var accounts = await clientAccountRepository.ListByClientAsync(client.Id, cancellationToken);
        var pendingInvitations = await invitationRepository.ListPendingByClientAsync(client.Id, timeProvider.GetUtcNow(), cancellationToken);
        var clientAppAccess = await AppAccessOfAsync(null, accounts, pendingInvitations, cancellationToken);
        var studentAppAccesses = new Dictionary<long, StudentAppAccessResponse>();
        foreach (var student in students)
        {
            studentAppAccesses[student.Id] = await AppAccessOfAsync(student.Id, accounts, pendingInvitations, cancellationToken);
        }

        var onTheTeam = await userRepository.ListOnTheTeamAsync([client.UserId, .. students.Select(student => student.UserId)], cancellationToken);
        var userIdsByStudent = students.ToDictionary(student => student.Id, student => student.UserId);
        var photoUrls = await userPhotoUrls.OfUsersAsync([client.UserId, .. userIdsByStudent.Values], cancellationToken);
        var details = ClientDetailsResponse.From(client, students, plans, clientAppAccess, studentAppAccesses);
        return details with
        {
            IsOnTheTeam = onTheTeam.Contains(client.UserId),
            PhotoUrl = photoUrls.GetValueOrDefault(client.UserId),
            Students =
            [
                .. details.Students.Select(student => student with
                {
                    IsOnTheTeam = onTheTeam.Contains(userIdsByStudent[student.Id]),
                    PhotoUrl = photoUrls.GetValueOrDefault(userIdsByStudent[student.Id]),
                }),
            ],
        };
    }

    private async Task<StudentAppAccessResponse> AppAccessOfAsync(
        long? studentId,
        IReadOnlyList<ClientAccount> accounts,
        IReadOnlyList<ClientInvitation> pendingInvitations,
        CancellationToken cancellationToken)
    {
        var accountIds = accounts.Where(account => account.StudentId == studentId).Select(account => account.User.AccountId).OfType<Guid>().ToList();
        var signInEmail = await SignInEmails.FirstAsync(identityService, accountIds, cancellationToken);
        var pendingInvitation = pendingInvitations.FirstOrDefault(invitation => invitation.StudentId == studentId);
        return StudentAppAccessResponse.From(accountIds.Count > 0, signInEmail, pendingInvitation);
    }
}
