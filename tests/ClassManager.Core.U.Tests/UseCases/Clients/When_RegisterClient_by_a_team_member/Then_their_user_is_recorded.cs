using ClassManager.Core.Domain.Clients;

namespace ClassManager.Core.U.Tests.UseCases.Clients.When_RegisterClient_by_a_team_member;

public sealed class Then_their_user_is_recorded
{
    [Fact]
    public async Task Then_their_user_is_recorded_Run()
    {
        var builder = new RegisterClientUseCaseBuilder();
        var userId = Guid.CreateVersion7();
        builder.CurrentUser.Setup(user => user.AccountId).Returns(Guid.CreateVersion7());
        builder.CurrentUser.Setup(user => user.UserId).Returns(userId);
        Client? addedClient = null;
        builder.Clients.Setup(repository => repository.Add(It.IsAny<Client>())).Callback<Client>(client => addedClient = client);

        await builder.Build().ExecuteAsync(RegisterClientUseCaseBuilder.ValidCommand(), CancellationToken.None);

        addedClient!.RegisteredByUserId.ShouldBe(userId);
    }
}
