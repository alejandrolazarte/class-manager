using ClassManager.Core.UseCases.StudentApp;

namespace ClassManager.Api.I.Tests.Endpoints.StudentAppNews.When_a_class_of_the_student_is_rescheduled;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_news_tells_the_new_time(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_news_tells_the_new_time_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        var nextWeek = InstructorScenario.ClassDate.AddDays(7);
        (await scenario.Instructors.Business.HttpClient.PutScheduleAsync(scenario.Instructors.InstructorClassGroup.Id, nextWeek, "20:30"))
            .EnsureSuccessStatusCode();

        var news = await scenario.Student.GetStudentAppNewsAsync();

        var changed = news!.Items.Single(item => item.Kind == StudentAppNewsKind.ClassChanged);
        changed.ClassStartTime.ShouldBe("20:30");
        changed.ClassDate.ShouldBe(nextWeek);
        changed.ClassName.ShouldBe(InstructorScenario.InstructorClassGroupName);
        changed.IsUnread.ShouldBeTrue();
    }
}
