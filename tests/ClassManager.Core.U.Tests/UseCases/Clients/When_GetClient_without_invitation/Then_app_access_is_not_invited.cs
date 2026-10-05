using ClassManager.Core.UseCases.Clients;

namespace ClassManager.Core.U.Tests.UseCases.Clients.When_GetClient_without_invitation;

public sealed class Then_app_access_is_not_invited
{
    [Fact]
    public async Task Then_app_access_is_not_invited_Run()
    {
        var builder = new GetClientUseCaseBuilder();

        var response = await builder.Build().ExecuteAsync(new GetClientQuery(builder.Client.Id), CancellationToken.None);

        response.Value!.AppAccess.ShouldBe(new StudentAppAccessResponse(StudentAppAccessStatus.NotInvited, null));
    }
}
