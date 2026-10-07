using ClassManager.Core.Domain.Accounts;

namespace ClassManager.Core.U.Tests.Domain.Accounts.When_OwnerAccount_is_created_with_invalid_email;

public sealed class Then_validation_fails
{
    [Fact]
    public void Then_validation_fails_Run()
    {
        var account = OwnerAccount.Create(TestData.OwnerFullName, "not-an-email", TestData.OwnerPassword, TestData.OwnerBirthDate, TestData.Today);

        account.Error!.FieldName.ShouldBe(nameof(OwnerAccount.Email));
    }
}
