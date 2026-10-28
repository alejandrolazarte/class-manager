namespace ClassManager.Api.I.Tests.Endpoints.StudentAppClassHistory.When_student_looks_back_at_the_month;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_today_and_later_classes_are_left_out(ApiFixture fixture)
{
    [Fact]
    public async Task Then_today_and_later_classes_are_left_out_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        var today = InstructorScenario.ClassDate;
        await scenario.EnrollInWeeklyClassSinceAsync(today.AddDays(-7));

        var history = await scenario.Student.GetClassHistoryAsync(scenario.Instructors.InstructorStudentId, ClassHistoryRequests.CurrentMonth);

        history!.Classes.Select(pastClass => pastClass.Date).ShouldBe([today.AddDays(-7)]);
    }
}
