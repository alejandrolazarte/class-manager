using ClassManager.Core.Domain.Announcements;
using ClassManager.Core.UseCases.StudentApp;

namespace ClassManager.Api.I.Tests.Endpoints.StudentAppNews.When_an_announcement_is_no_longer_visible;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_student_does_not_see_it(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_student_does_not_see_it_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        var publishedAt = BusinessApiFactory.Now.AddDays(-10);
        var publishedOn = DateOnly.FromDateTime(publishedAt.UtcDateTime);
        await using (var context = fixture.CreateDbContext(scenario.Instructors.Business.Business.Id))
        {
            context.Announcements.Add(Announcement.Create(
                "Feriado del lunes", null, publishedAt, publishedOn, visibleUntil: publishedOn.AddDays(5)).Value!);
            await context.SaveChangesAsync();
        }

        var news = await scenario.Student.GetStudentAppNewsAsync();

        news!.Items.ShouldNotContain(item => item.Kind == StudentAppNewsKind.Announcement);
    }
}
