using ClassManager.Core.UseCases.Families;

namespace ClassManager.Api.I.Tests.Endpoints.FamilyNews.When_a_class_of_two_siblings_is_cancelled;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_news_mentions_both_once(ApiFixture fixture)
{
    private const string SiblingFullName = "Sofía Pérez";

    [Fact]
    public async Task Then_the_news_mentions_both_once_Run()
    {
        var scenario = await fixture.SeedFamilyScenarioAsync();
        var owner = scenario.Coaches.Business.HttpClient;
        var siblingId = (await owner.AddStudentAsync(scenario.FamilyId, SiblingFullName)).Id;
        await owner.EnrollAsync(scenario.Coaches.CoachClassGroup.Id, siblingId, CoachScenario.ClassDate);
        var nextWeek = CoachScenario.ClassDate.AddDays(7);
        (await owner.PutSessionCancellationAsync(scenario.Coaches.CoachClassGroup.Id, nextWeek, "Feriado")).EnsureSuccessStatusCode();

        var news = await scenario.Family.GetFamilyNewsAsync();

        var cancelled = news!.Items.Single(item => item.Kind == FamilyNewsKind.ClassCancelled);
        cancelled.StudentFullNames.ShouldBe([CoachScenario.CoachStudentFullName, SiblingFullName], ignoreOrder: true);
    }
}
