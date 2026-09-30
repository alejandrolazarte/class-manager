using ClassManager.Core.UseCases.Families;

namespace ClassManager.Api.I.Tests.Endpoints.FamilyNews.When_a_class_of_the_student_is_cancelled;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_news_tells_the_date(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_news_tells_the_date_Run()
    {
        var scenario = await fixture.SeedFamilyScenarioAsync();
        var nextWeek = CoachScenario.ClassDate.AddDays(7);
        (await scenario.Coaches.Business.HttpClient.PutSessionCancellationAsync(scenario.Coaches.CoachClassGroup.Id, nextWeek, "Feriado"))
            .EnsureSuccessStatusCode();

        var news = await scenario.Family.GetFamilyNewsAsync();

        var cancelled = news!.Items.Single(item => item.Kind == FamilyNewsKind.ClassCancelled);
        cancelled.ClassDate.ShouldBe(nextWeek);
        cancelled.ClassName.ShouldBe(CoachScenario.CoachClassGroupName);
        cancelled.Body.ShouldBe("Feriado");
    }
}
