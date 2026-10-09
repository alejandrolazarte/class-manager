using ClassManager.Core.UseCases.StudentApp;

namespace ClassManager.Api.I.Tests.Endpoints.StudentAppNews.When_a_class_of_the_student_gets_a_substitute;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_news_names_the_substitute(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_news_names_the_substitute_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        var nextWeek = InstructorScenario.ClassDate.AddDays(7);
        var substitute = await scenario.Instructors.Business.HttpClient.CreateInstructorAsync("Paula Ruiz");
        (await scenario.Instructors.Business.HttpClient.PutSubstituteAsync(scenario.Instructors.InstructorClassGroup.Id, nextWeek, substitute.Id))
            .EnsureSuccessStatusCode();

        var news = await scenario.Student.GetStudentAppNewsAsync();

        var changed = news!.Items.Single(item => item.Kind == StudentAppNewsKind.ClassChanged);
        changed.InstructorFullName.ShouldBe("Paula Ruiz");
        changed.ClassStartTime.ShouldBeNull();
        changed.ClassDate.ShouldBe(nextWeek);
    }
}
