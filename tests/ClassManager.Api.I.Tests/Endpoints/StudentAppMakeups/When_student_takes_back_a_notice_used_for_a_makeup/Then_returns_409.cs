using ClassManager.Core.Domain.Sessions;

namespace ClassManager.Api.I.Tests.Endpoints.StudentAppMakeups.When_student_takes_back_a_notice_used_for_a_makeup;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_409(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_409_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        await scenario.NoticeAndBookOtherClassAsync();

        using var response = await scenario.Student.DeleteAbsenceAsync(
            scenario.Coaches.CoachStudentId, scenario.Coaches.CoachClassGroup.Id, CoachScenario.ClassDate);

        response.StatusCode.ShouldBe(HttpStatusCode.Conflict);
        (await response.Content.ReadAsStringAsync()).ShouldContain(SessionErrorCodes.MakeupCreditInUse);
    }
}
