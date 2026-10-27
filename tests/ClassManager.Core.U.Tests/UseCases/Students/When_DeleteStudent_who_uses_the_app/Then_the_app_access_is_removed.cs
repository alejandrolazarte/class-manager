namespace ClassManager.Core.U.Tests.UseCases.Students.When_DeleteStudent_who_uses_the_app;

public sealed class Then_the_app_access_is_removed
{
    [Fact]
    public async Task Then_the_app_access_is_removed_Run()
    {
        var builder = new DeleteStudentUseCaseBuilder();
        var account = builder.WithStudentAppAccount();

        await builder.Build().ExecuteAsync(builder.Command(), CancellationToken.None);

        builder.ClientAccounts.Verify(repository => repository.Remove(account), Times.Once);
    }
}
