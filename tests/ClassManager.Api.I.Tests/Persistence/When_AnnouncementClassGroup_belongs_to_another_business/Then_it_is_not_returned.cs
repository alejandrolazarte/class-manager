using ClassManager.Core.UseCases.Announcements;
using Microsoft.EntityFrameworkCore;

namespace ClassManager.Api.I.Tests.Persistence.When_AnnouncementClassGroup_belongs_to_another_business;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_is_not_returned(ApiFixture fixture)
{
    [Fact]
    public async Task Then_it_is_not_returned_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var otherBusiness = await fixture.SeedInstructorScenarioAsync();
        var otherAnnouncement = await otherBusiness.Business.HttpClient.PublishAnnouncementAsync(new CreateAnnouncementCommand(
            "Cambia el horario", null, ClassGroupIds: [otherBusiness.InstructorClassGroup.Id]));

        await using var context = fixture.CreateDbContext(business.Business.Id);

        (await context.AnnouncementClassGroups.AnyAsync(link => link.AnnouncementId == otherAnnouncement.Id)).ShouldBeFalse();
    }
}
