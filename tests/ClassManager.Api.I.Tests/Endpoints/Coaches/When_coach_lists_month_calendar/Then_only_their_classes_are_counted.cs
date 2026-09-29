namespace ClassManager.Api.I.Tests.Endpoints.Coaches.When_coach_lists_month_calendar;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_only_their_classes_are_counted(ApiFixture fixture)
{
    [Fact]
    public async Task Then_only_their_classes_are_counted_Run()
    {
        var scenario = await fixture.SeedCoachScenarioAsync();

        var calendar = await scenario.Coach.GetMonthCalendarAsync("2026-09");

        calendar!.Days.Single(day => day.Date == CoachScenario.ClassDate).ClassCount.ShouldBe(1);
    }
}
