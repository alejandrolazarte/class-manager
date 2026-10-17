using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.UseCases.Accounts;

namespace ClassManager.Core.U.Tests.UseCases.Accounts.When_DeleteMyAccount_has_the_wrong_password;

public sealed class Then_nothing_is_deleted
{
    [Fact]
    public async Task Then_nothing_is_deleted_Run()
    {
        var builder = new DeleteMyAccountUseCaseBuilder();

        var response = await builder.Build().ExecuteAsync(new DeleteMyAccountCommand("not my password"), CancellationToken.None);

        response.Error!.Code.ShouldBe(AuthenticationErrorCodes.InvalidCurrentPassword);
        response.Error.FieldName.ShouldBe(nameof(DeleteMyAccountCommand.CurrentPassword));
        builder.VerifyNothingWasDeleted();
    }
}
