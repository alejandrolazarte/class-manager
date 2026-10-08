using ClassManager.Core.Domain.Businesses;
using ClassManager.Core.Domain.Sessions;

namespace ClassManager.Api.I.Tests.Endpoints.Viewers.When_viewer_records_attendance;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_403(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_403_Run()
    {
        var scenario = await fixture.SeedInstructorScenarioAsync();
        using var viewer = await fixture.SeedMemberAsync(scenario.Business.Business.Id, BusinessRole.Viewer);

        using var response = await viewer.PutAttendanceAsync(
            scenario.InstructorClassGroup.Id, InstructorScenario.ClassDate, scenario.InstructorStudentId, AttendanceStatus.Present);

        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }
}
