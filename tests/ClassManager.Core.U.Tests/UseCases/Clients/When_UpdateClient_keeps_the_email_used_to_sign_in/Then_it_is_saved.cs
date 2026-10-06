namespace ClassManager.Core.U.Tests.UseCases.Clients.When_UpdateClient_keeps_the_email_used_to_sign_in;

public sealed class Then_it_is_saved
{
    [Fact]
    public async Task Then_it_is_saved_Run()
    {
        var builder = new UpdateClientUseCaseBuilder();
        builder.WithStudentAppAccount();

        var response = await builder.Build().ExecuteAsync(
            builder.ValidCommand() with { Email = UpdateClientUseCaseBuilder.SignInEmail }, CancellationToken.None);

        response.Value!.Email.ShouldBe(UpdateClientUseCaseBuilder.SignInEmail);
    }
}
