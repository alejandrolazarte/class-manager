using ClassManager.Core.UseCases.Clients;

namespace ClassManager.Core.U.Tests.UseCases.Clients.When_GetClient_with_student_app_account;

public sealed class Then_app_access_is_active
{
    [Fact]
    public async Task Then_app_access_is_active_Run()
    {
        var builder = new GetClientUseCaseBuilder();
        builder.WithStudentAppAccount();
        builder.WithPendingInvitation();

        var response = await builder.Build().ExecuteAsync(new GetClientQuery(builder.Client.Id), CancellationToken.None);

        response.Value!.AppAccess.ShouldBe(new StudentAppAccessResponse(StudentAppAccessStatus.Active, null));
    }
}
