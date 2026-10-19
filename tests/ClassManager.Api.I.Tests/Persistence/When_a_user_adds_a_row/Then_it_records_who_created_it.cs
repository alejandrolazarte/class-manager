using Microsoft.EntityFrameworkCore;

namespace ClassManager.Api.I.Tests.Persistence.When_a_user_adds_a_row;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_records_who_created_it(ApiFixture fixture)
{
    [Fact]
    public async Task Then_it_records_who_created_it_Run()
    {
        var business = await fixture.SeedBusinessAsync();

        var client = await business.HttpClient.RegisterClientAsync();

        await using var context = fixture.CreateDbContext(business.Business.Id);
        var ownerUserId = await context.Users.Where(user => user.AccountId == business.OwnerAccountId).Select(user => user.Id).SingleAsync();
        var saved = await context.Clients.AsNoTracking().SingleAsync(row => row.Id == client.Id);
        (saved.CreatedByUserId, saved.UpdatedByUserId, saved.UpdatedAt).ShouldBe((ownerUserId, ownerUserId, BusinessApiFactory.Now));
    }
}
