using ClassManager.Core.Domain.Announcements;
using ClassManager.Core.UseCases.Announcements;
using Microsoft.EntityFrameworkCore;

namespace ClassManager.Api.I.Tests.Persistence.When_AnnouncementReply_belongs_to_another_business;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_is_not_returned(ApiFixture fixture)
{
    [Fact]
    public async Task Then_it_is_not_returned_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var otherScenario = await fixture.SeedStudentAppScenarioAsync();
        var otherAnnouncement = await otherScenario.Instructors.Business.HttpClient.PublishAnnouncementAsync(
            new CreateAnnouncementCommand("Muestra de fin de año", null, AnnouncementCategory.Event));
        (await otherScenario.Student.PutAnnouncementReplyAsync(otherAnnouncement.Id, isGoing: true)).EnsureSuccessStatusCode();

        await using var context = fixture.CreateDbContext(business.Business.Id);

        (await context.AnnouncementReplies.AnyAsync(reply => reply.AnnouncementId == otherAnnouncement.Id)).ShouldBeFalse();
    }
}
