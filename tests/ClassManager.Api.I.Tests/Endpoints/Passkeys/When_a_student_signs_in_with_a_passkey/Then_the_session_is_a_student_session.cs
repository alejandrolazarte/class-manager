using ClassManager.Core.Abstractions.Security;

namespace ClassManager.Api.I.Tests.Endpoints.Passkeys.When_a_student_signs_in_with_a_passkey;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_session_is_a_student_session(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_session_is_a_student_session_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        using var authenticator = new SoftwareAuthenticator();
        await scenario.Student.AddPasskeyAsync(authenticator, StudentAppRequests.StudentAppPassword);
        using var anonymous = fixture.ApiFactory.CreateClient();

        using var response = await anonymous.SignInWithPasskeyAsync(authenticator);

        (await response.ReadTokensAsync()).Kind.ShouldBe(AccountKinds.Student);
    }
}
