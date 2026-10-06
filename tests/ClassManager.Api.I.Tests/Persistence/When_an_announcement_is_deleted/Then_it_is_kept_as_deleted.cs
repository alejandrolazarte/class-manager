using ClassManager.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ClassManager.Api.I.Tests.Persistence.When_an_announcement_is_deleted;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_is_kept_as_deleted(ApiFixture fixture)
{
    [Fact]
    public async Task Then_it_is_kept_as_deleted_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var announcement = await business.HttpClient.PublishAnnouncementAsync();

        (await business.HttpClient.DeleteAsync(new Uri($"{ApiRoutes.Announcements}/{announcement.Id}", UriKind.Relative))).EnsureSuccessStatusCode();

        await using var context = fixture.CreateDbContext(business.Business.Id);
        (await context.Announcements.AnyAsync(row => row.Id == announcement.Id)).ShouldBeFalse();
        (await context.Announcements
            .IgnoreQueryFilters([SoftDeleteModelBuilderExtensions.SoftDeleteQueryFilter])
            .SingleAsync(row => row.Id == announcement.Id))
            .DeletedOn.ShouldBe(BusinessApiFactory.Now);
    }
}
