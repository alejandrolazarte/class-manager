using ClassManager.Core.UseCases.StudentApp;

namespace ClassManager.Api.I.Tests.Endpoints.StudentAppNews.When_instructor_comments_a_class;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_news_includes_the_comment(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_news_includes_the_comment_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        (await scenario.Instructors.Instructor.PutFeedbackAsync(
            scenario.Instructors.InstructorClassGroup.Id, InstructorScenario.ClassDate, scenario.Instructors.InstructorStudentId, "Muy buen ritmo"))
            .EnsureSuccessStatusCode();

        var news = await scenario.Student.GetStudentAppNewsAsync();

        var comment = news!.Items.Single(item => item.Kind == StudentAppNewsKind.InstructorFeedback);
        comment.InstructorFullName.ShouldBe(InstructorScenario.InstructorFullName);
        comment.StudentFullNames.ShouldBe([InstructorScenario.InstructorStudentFullName]);
        comment.Body.ShouldBe("Muy buen ritmo");
    }
}
