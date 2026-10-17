using ClassManager.Core.UseCases.Accounts;
using ClassManager.Core.UseCases.Branches;

namespace ClassManager.Core.U.Tests.UseCases.Accounts.When_DeleteMyAccount_is_asked_by_the_only_brand_owner;

public sealed class Then_it_is_rejected
{
    [Fact]
    public async Task Then_it_is_rejected_Run()
    {
        var builder = new DeleteMyAccountUseCaseBuilder().WithCorrectPassword().AsTheOnlyBrandOwner();

        var response = await builder.Build().ExecuteAsync(
            new DeleteMyAccountCommand(DeleteMyAccountUseCaseBuilder.CurrentPassword), CancellationToken.None);

        response.Error!.Code.ShouldBe(BranchErrorCodes.LastBrandOwner);
        builder.VerifyNothingWasDeleted();
    }
}
