using ClassManager.Core.UseCases.StudentApp;

namespace ClassManager.Api.I.Tests.Endpoints.StudentAppNews.When_a_class_of_the_student_is_cancelled;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_news_tells_the_date(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_news_tells_the_date_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        var nextWeek = CoachScenario.ClassDate.AddDays(7);
        (await scenario.Coaches.Business.HttpClient.PutSessionCancellationAsync(scenario.Coaches.CoachClassGroup.Id, nextWeek, "Feriado"))
            .EnsureSuccessStatusCode();

        var news = await scenario.Student.GetStudentAppNewsAsync();

        var cancelled = news!.Items.Single(item => item.Kind == StudentAppNewsKind.ClassCancelled);
        cancelled.ClassDate.ShouldBe(nextWeek);
        cancelled.ClassName.ShouldBe(CoachScenario.CoachClassGroupName);
        cancelled.Body.ShouldBe("Feriado");
    }
}
