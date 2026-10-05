using ClassManager.Core.UseCases.Clients;

namespace ClassManager.Core.U.Tests.UseCases.Clients.When_GetClient_with_pending_invitation;

public sealed class Then_app_access_is_invited_with_its_email
{
    [Fact]
    public async Task Then_app_access_is_invited_with_its_email_Run()
    {
        var builder = new GetClientUseCaseBuilder();
        builder.WithPendingInvitation();

        var response = await builder.Build().ExecuteAsync(new GetClientQuery(builder.Client.Id), CancellationToken.None);

        response.Value!.AppAccess.ShouldBe(new StudentAppAccessResponse(StudentAppAccessStatus.Invited, GetClientUseCaseBuilder.InvitedEmail));
    }
}
