using ClassManager.Core.UseCases.Clients;
using Microsoft.EntityFrameworkCore;

namespace ClassManager.Api.I.Tests.Persistence.When_a_client_is_edited;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_user_follows(ApiFixture fixture)
{
    private const string NewFullName = "Ana María Pérez";
    private const string NewEmail = "ana.maria@example.com";

    [Fact]
    public async Task Then_the_user_follows_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var client = await business.HttpClient.RegisterClientAsync();

        using var response = await business.HttpClient.PutAsJsonAsync(
            $"{ApiRoutes.Clients}/{client.Id}",
            new UpdateClientRequest(NewFullName, "11 5555-0101", NewEmail, null),
            ApiRequests.JsonOptions);

        await using var context = fixture.CreateDbContext(business.Business.Id);
        var userId = (await context.Clients.AsNoTracking().SingleAsync(row => row.Id == client.Id)).UserId;
        var user = await context.Users.AsNoTracking().SingleAsync(row => row.Id == userId);
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        (user.FullName, user.Email, user.PhoneNumber?.Value).ShouldBe((NewFullName, NewEmail, "+541155550101"));
    }
}
