namespace ClassManager.Api.I.Tests.Endpoints.StudentAppClassHistory.When_student_joined_the_class_during_the_month;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_classes_before_joining_are_left_out(ApiFixture fixture)
{
    [Fact]
    public async Task Then_classes_before_joining_are_left_out_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        var today = InstructorScenario.ClassDate;
        await scenario.EnrollInWeeklyClassSinceAsync(today.AddDays(-14));

        var history = await scenario.Student.GetClassHistoryAsync(scenario.Instructors.InstructorStudentId, ClassHistoryRequests.CurrentMonth);

        history!.Classes.Select(pastClass => pastClass.Date).ShouldBe([today.AddDays(-14), today.AddDays(-7)]);
    }
}
