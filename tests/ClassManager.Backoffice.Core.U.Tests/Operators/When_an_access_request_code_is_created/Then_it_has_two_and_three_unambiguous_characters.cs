namespace ClassManager.Backoffice.Core.U.Tests.Operators.When_an_access_request_code_is_created;

public sealed class Then_it_has_two_and_three_unambiguous_characters
{
    private const string CodePattern = "^[A-HJKMNP-Z2-9]{2}-[A-HJKMNP-Z2-9]{3}$";

    [Fact]
    public void Then_it_has_two_and_three_unambiguous_characters_Run()
    {
        var code = AccessRequestCodes.Create();

        code.ShouldMatch(CodePattern);
    }
}
