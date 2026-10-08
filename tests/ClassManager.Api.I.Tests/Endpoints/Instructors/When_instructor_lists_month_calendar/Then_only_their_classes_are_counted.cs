namespace ClassManager.Api.I.Tests.Endpoints.Instructors.When_instructor_lists_month_calendar;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_only_their_classes_are_counted(ApiFixture fixture)
{
    [Fact]
    public async Task Then_only_their_classes_are_counted_Run()
    {
        var scenario = await fixture.SeedInstructorScenarioAsync();

        var calendar = await scenario.Instructor.GetMonthCalendarAsync("2026-09");

        calendar!.Days.Single(day => day.Date == InstructorScenario.ClassDate).ClassCount.ShouldBe(1);
    }
}
