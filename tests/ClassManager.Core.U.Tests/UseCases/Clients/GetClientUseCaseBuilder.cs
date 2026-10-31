using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Abstractions.Time;
using ClassManager.Core.Domain.Clients;
using ClassManager.Core.UseCases.Clients;
using Microsoft.Extensions.Time.Testing;

namespace ClassManager.Core.U.Tests.UseCases.Clients;

internal sealed class GetClientUseCaseBuilder
{
    public const string InvitedEmail = "ana@example.com";
    public const string SignInEmail = "ana.app@example.com";

    public Mock<IClientRepository> Clients { get; } = new();
    public Mock<IStudentRepository> Students { get; } = new();
    public Mock<IStudentPlanRepository> StudentPlans { get; } = new();
    public Mock<IClassPackRepository> ClassPacks { get; } = new();
    public Mock<IClientAccountRepository> ClientAccounts { get; } = new();
    public Mock<IClientInvitationRepository> Invitations { get; } = new();
    public Mock<IIdentityService> Identity { get; } = new();
    public Mock<IUserRepository> Users { get; } = new();
    public Client Client { get; } = TestData.Client();

    public GetClientUseCaseBuilder()
    {
        Clients.Setup(repository => repository.GetByIdAsync(Client.Id, It.IsAny<CancellationToken>())).ReturnsAsync(Client);
        Students.Setup(repository => repository.ListByClientAsync(Client.Id, It.IsAny<CancellationToken>())).ReturnsAsync([]);
        ClientAccounts.Setup(repository => repository.ListByClientAsync(Client.Id, It.IsAny<CancellationToken>())).ReturnsAsync([]);
        Invitations.Setup(repository => repository.ListPendingByClientAsync(Client.Id, It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>())).ReturnsAsync([]);
        StudentPlans
            .Setup(repository => repository.ListByClientsAsync(It.IsAny<IReadOnlyCollection<long>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);
        ClassPacks.Setup(repository => repository.ListAsync(It.IsAny<bool>(), It.IsAny<CancellationToken>())).ReturnsAsync([]);
        ClassPacks.Setup(repository => repository.ListPriceChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync([]);
    }

    public void WithPendingInvitation() =>
        Invitations
            .Setup(repository => repository.ListPendingByClientAsync(Client.Id, TestData.Now, It.IsAny<CancellationToken>()))
            .ReturnsAsync([ClientInvitation.Create(Client.Id, InvitedEmail, "token-hash", TestIds.Next(), TestData.Now).Value!.WithId()]);

    public void WithStudentAppAccount()
    {
        var userId = Guid.CreateVersion7();
        ClientAccounts.Setup(repository => repository.ListByClientAsync(Client.Id, It.IsAny<CancellationToken>())).ReturnsAsync([ClientAccount.Create(Client.Id, TestData.UserOfAccount(userId), TestData.Now).WithId()]);
        Identity
            .Setup(service => service.ListAccountsAsync(It.IsAny<IReadOnlyCollection<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([new UserAccount(userId, SignInEmail, Client.User.FullName)]);
    }

    public GetClientUseCase Build()
    {
        Users
            .Setup(repository => repository.ListOnTheTeamAsync(It.IsAny<IReadOnlyCollection<long>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new HashSet<long>());
        return new(
            Clients.Object,
            Students.Object,
            StudentPlans.Object,
            ClassPacks.Object,
            ClientAccounts.Object,
            Invitations.Object,
            Identity.Object,
            Users.Object,
            new Mock<IBusinessCalendarService>().Object,
            new EveryAccessScopes(),
            new NoUserPhotoUrls(),
            new FakeTimeProvider(TestData.Now));
    }
}
