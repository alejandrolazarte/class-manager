using Microsoft.EntityFrameworkCore;

namespace ClassManager.Api.I.Tests.Persistence.When_AnnouncementView_belongs_to_another_business;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_is_not_returned(ApiFixture fixture)
{
    [Fact]
    public async Task Then_it_is_not_returned_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var otherScenario = await fixture.SeedStudentAppScenarioAsync();
        var otherAnnouncement = await otherScenario.Instructors.Business.HttpClient.PublishAnnouncementAsync();
        (await otherScenario.Student.PutStudentAppNewsSeenAsync()).EnsureSuccessStatusCode();

        await using var context = fixture.CreateDbContext(business.Business.Id);

        (await context.AnnouncementViews.AnyAsync(view => view.AnnouncementId == otherAnnouncement.Id)).ShouldBeFalse();
    }
}
