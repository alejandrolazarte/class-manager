using ClassManager.Core.Abstractions.Security;

namespace ClassManager.Api.I.Tests.Endpoints.Accounts.When_an_email_change_has_the_wrong_password;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_400(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_400_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();

        using var response = await scenario.Student.PostEmailChangeAsync(EmailChangeRequests.UniqueNewEmail(), AuthenticationRequests.WrongPassword);

        (await response.ReadProblemAsync()).GetProperty("code").GetString().ShouldBe(AuthenticationErrorCodes.InvalidCurrentPassword);
    }
}
