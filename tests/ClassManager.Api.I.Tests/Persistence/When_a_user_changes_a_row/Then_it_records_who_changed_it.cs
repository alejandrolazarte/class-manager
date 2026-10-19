using ClassManager.Core.Domain.Businesses;
using ClassManager.Core.UseCases.Clients;
using Microsoft.EntityFrameworkCore;

namespace ClassManager.Api.I.Tests.Persistence.When_a_user_changes_a_row;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_records_who_changed_it(ApiFixture fixture)
{
    private const string SeededMemberFullName = "Miembro sembrado";
    private const string NewFullName = "Ana María Pérez";

    [Fact]
    public async Task Then_it_records_who_changed_it_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var client = await business.HttpClient.RegisterClientAsync();
        using var otherOwner = await fixture.SeedMemberAsync(business.Business.Id, BusinessRole.BranchOwner);

        using var response = await otherOwner.PutAsJsonAsync(
            $"{ApiRoutes.Clients}/{client.Id}",
            new UpdateClientRequest(NewFullName, "11 5555-0101", null, null),
            ApiRequests.JsonOptions);

        await using var context = fixture.CreateDbContext(business.Business.Id);
        var ownerUserId = await context.Users.Where(user => user.AccountId == business.OwnerAccountId).Select(user => user.Id).SingleAsync();
        var otherOwnerUserId = await context.Users.Where(user => user.FullName == SeededMemberFullName).Select(user => user.Id).SingleAsync();
        var userId = await context.Clients.Where(row => row.Id == client.Id).Select(row => row.UserId).SingleAsync();
        var contact = await context.Users.AsNoTracking().SingleAsync(row => row.Id == userId);
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        (contact.CreatedByUserId, contact.UpdatedByUserId).ShouldBe((ownerUserId, otherOwnerUserId));
    }
}
