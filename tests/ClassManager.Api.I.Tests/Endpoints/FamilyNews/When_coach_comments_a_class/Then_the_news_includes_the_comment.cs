using ClassManager.Core.UseCases.Families;

namespace ClassManager.Api.I.Tests.Endpoints.FamilyNews.When_coach_comments_a_class;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_news_includes_the_comment(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_news_includes_the_comment_Run()
    {
        var scenario = await fixture.SeedFamilyScenarioAsync();
        (await scenario.Coaches.Coach.PutFeedbackAsync(
            scenario.Coaches.CoachClassGroup.Id, CoachScenario.ClassDate, scenario.Coaches.CoachStudentId, "Muy buen ritmo"))
            .EnsureSuccessStatusCode();

        var news = await scenario.Family.GetFamilyNewsAsync();

        var comment = news!.Items.Single(item => item.Kind == FamilyNewsKind.CoachFeedback);
        comment.InstructorFullName.ShouldBe(CoachScenario.CoachFullName);
        comment.StudentFullNames.ShouldBe([CoachScenario.CoachStudentFullName]);
        comment.Body.ShouldBe("Muy buen ritmo");
    }
}
