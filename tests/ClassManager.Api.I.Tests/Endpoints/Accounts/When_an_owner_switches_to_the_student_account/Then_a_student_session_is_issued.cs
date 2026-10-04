using ClassManager.Core.Abstractions.Security;

namespace ClassManager.Api.I.Tests.Endpoints.Accounts.When_an_owner_switches_to_the_student_account;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_a_student_session_is_issued(ApiFixture fixture)
{
    [Fact]
    public async Task Then_a_student_session_is_issued_Run()
    {
        var seeded = await fixture.SeedOwnerWhoIsAlsoStudentAsync();
        using var client = fixture.CreateClientWithToken(seeded.OwnerTokens.AccessToken);

        using var response = await client.PostSwitchAccountAsync(
            seeded.OwnerTokens.RefreshToken, seeded.Student.Coaches.Business.Business.Id, AccountKinds.Student);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var tokens = await response.ReadTokensAsync();
        tokens.Kind.ShouldBe(AccountKinds.Student);
        using var studentClient = fixture.CreateClientWithToken(tokens.AccessToken);
        (await studentClient.GetStudentAppHomeAsync())!.Students.ShouldHaveSingleItem().FullName.ShouldBe(CoachScenario.CoachStudentFullName);
    }
}
