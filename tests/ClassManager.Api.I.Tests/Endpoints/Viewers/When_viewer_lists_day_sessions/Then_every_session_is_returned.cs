using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Api.I.Tests.Endpoints.Viewers.When_viewer_lists_day_sessions;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_every_session_is_returned(ApiFixture fixture)
{
    [Fact]
    public async Task Then_every_session_is_returned_Run()
    {
        var scenario = await fixture.SeedInstructorScenarioAsync();
        using var viewer = await fixture.SeedMemberAsync(scenario.Business.Business.Id, BusinessRole.Viewer);

        var sessions = await viewer.GetDayAsync(InstructorScenario.ClassDate);

        sessions!.Select(session => session.ClassGroupName).ShouldBe([InstructorScenario.InstructorClassGroupName, InstructorScenario.OtherClassGroupName]);
    }
}
