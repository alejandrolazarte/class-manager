using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Time;
using ClassManager.Core.Domain.Clients;
using ClassManager.Core.UseCases.Clients;
using Microsoft.Extensions.Time.Testing;

namespace ClassManager.Core.U.Tests.UseCases.Clients;

internal sealed class GetClientUseCaseBuilder
{
    public const string InvitedEmail = "ana@example.com";

    public Mock<IClientRepository> Clients { get; } = new();
    public Mock<IStudentRepository> Students { get; } = new();
    public Mock<IFeeScheduleRepository> FeeSchedules { get; } = new();
    public Mock<IClientAccountRepository> ClientAccounts { get; } = new();
    public Mock<IClientInvitationRepository> Invitations { get; } = new();
    public Client Client { get; } = TestData.Client();

    public GetClientUseCaseBuilder()
    {
        Clients.Setup(repository => repository.GetByIdAsync(Client.Id, It.IsAny<CancellationToken>())).ReturnsAsync(Client);
        Students.Setup(repository => repository.ListByClientAsync(Client.Id, It.IsAny<CancellationToken>())).ReturnsAsync([]);
        FeeSchedules
            .Setup(repository => repository.ListClientPlanChangesAsync(It.IsAny<IReadOnlyCollection<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);
    }

    public void WithPendingInvitation() =>
        Invitations
            .Setup(repository => repository.FindLatestPendingByClientAsync(Client.Id, TestData.Now, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ClientInvitation.Create(Client.Id, InvitedEmail, "token-hash", Guid.CreateVersion7(), TestData.Now).Value);

    public void WithStudentAppAccount() =>
        ClientAccounts.Setup(repository => repository.HasAccountAsync(Client.Id, It.IsAny<CancellationToken>())).ReturnsAsync(true);

    public GetClientUseCase Build() =>
        new(
            Clients.Object,
            Students.Object,
            FeeSchedules.Object,
            ClientAccounts.Object,
            Invitations.Object,
            new Mock<IBusinessCalendarService>().Object,
            new EveryAccessScopes(),
            new FakeTimeProvider(TestData.Now));
}
