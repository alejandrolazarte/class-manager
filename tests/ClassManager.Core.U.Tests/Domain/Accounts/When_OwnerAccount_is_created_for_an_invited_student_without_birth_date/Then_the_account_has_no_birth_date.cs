using ClassManager.Core.Domain.Accounts;

namespace ClassManager.Core.U.Tests.Domain.Accounts.When_OwnerAccount_is_created_for_an_invited_student_without_birth_date;

public sealed class Then_the_account_has_no_birth_date
{
    [Fact]
    public void Then_the_account_has_no_birth_date_Run()
    {
        var account = OwnerAccount.CreateForInvitedStudent(TestData.OwnerFullName, TestData.OwnerEmail, TestData.OwnerPassword, null);

        account.Value!.BirthDate.ShouldBeNull();
    }
}
