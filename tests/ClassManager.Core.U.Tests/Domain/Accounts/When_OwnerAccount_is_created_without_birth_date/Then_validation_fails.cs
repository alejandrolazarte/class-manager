using ClassManager.Core.Domain.Accounts;

namespace ClassManager.Core.U.Tests.Domain.Accounts.When_OwnerAccount_is_created_without_birth_date;

public sealed class Then_validation_fails
{
    [Fact]
    public void Then_validation_fails_Run()
    {
        var account = OwnerAccount.Create(TestData.OwnerFullName, TestData.OwnerEmail, TestData.OwnerPassword, null, TestData.Today);

        account.Error!.FieldName.ShouldBe(nameof(OwnerAccount.BirthDate));
    }
}
