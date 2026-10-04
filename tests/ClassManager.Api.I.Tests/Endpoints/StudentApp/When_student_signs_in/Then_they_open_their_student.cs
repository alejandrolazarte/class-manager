using ClassManager.Core.Abstractions.Security;

namespace ClassManager.Api.I.Tests.Endpoints.StudentApp.When_student_signs_in;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_they_open_their_student(ApiFixture fixture)
{
    [Fact]
    public async Task Then_they_open_their_student_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        using var anonymous = fixture.ApiFactory.CreateClient();

        var tokens = await anonymous.SignInAsync(scenario.Email, StudentAppRequests.StudentAppPassword);

        tokens.Kind.ShouldBe(AccountKinds.Student);
        using var student = fixture.CreateClientWithToken(tokens.AccessToken);
        (await student.GetStudentAppHomeAsync())!.ClientFullName.ShouldNotBeEmpty();
    }
}
