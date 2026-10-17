using ClassManager.Core.UseCases.Accounts;

namespace ClassManager.Core.U.Tests.UseCases.Accounts.When_DeleteMyAccount_has_no_password;

public sealed class Then_validation_fails
{
    [Fact]
    public async Task Then_validation_fails_Run()
    {
        var builder = new DeleteMyAccountUseCaseBuilder().WithCorrectPassword();

        var response = await builder.Build().ExecuteAsync(new DeleteMyAccountCommand(string.Empty), CancellationToken.None);

        response.Error!.Kind.ShouldBe(ErrorKind.Validation);
        response.Error.FieldName.ShouldBe(nameof(DeleteMyAccountCommand.CurrentPassword));
        builder.VerifyNothingWasDeleted();
    }
}
