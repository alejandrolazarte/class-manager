using ClassManager.Core.Abstractions.Security;

namespace ClassManager.Api.I.Tests.Endpoints.StudentApp.When_student_accepts_the_invitation;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_session_kind_is_student(ApiFixture fixture)
{
    [Fact]
    public async Task Then_session_kind_is_student_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();

        scenario.Tokens.Kind.ShouldBe(AccountKinds.Student);
    }
}
