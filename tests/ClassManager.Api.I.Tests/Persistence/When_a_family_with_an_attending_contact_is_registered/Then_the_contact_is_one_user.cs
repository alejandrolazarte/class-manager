using ClassManager.Core.UseCases.Clients;
using ClassManager.Core.UseCases.Students;
using Microsoft.EntityFrameworkCore;

namespace ClassManager.Api.I.Tests.Persistence.When_a_family_with_an_attending_contact_is_registered;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_contact_is_one_user(ApiFixture fixture)
{
    private const string ChildFullName = "Tomás Pérez";

    [Fact]
    public async Task Then_the_contact_is_one_user_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        using var response = await business.HttpClient.PostAsJsonAsync(
            ApiRoutes.Clients,
            new RegisterClientCommand(
                ApiRequests.ClientFullName,
                "11 4321-8765",
                "ana@example.com",
                null,
                [new NewStudent(ApiRequests.ClientFullName, null, null), new NewStudent(ChildFullName, new DateOnly(2014, 3, 14), null)]),
            ApiRequests.JsonOptions);
        var family = (await response.Content.ReadFromJsonAsync<ClientDetailsResponse>(ApiRequests.JsonOptions))!;

        await using var context = fixture.CreateDbContext(business.Business.Id);
        var client = await context.Clients.AsNoTracking().SingleAsync(client => client.Id == family.Id);
        var students = await context.Students.AsNoTracking().Where(student => student.ClientId == family.Id).ToListAsync();
        var users = await context.Users.AsNoTracking().OrderBy(user => user.FullName).ToListAsync();

        users.Select(user => (user.FullName, user.Email, user.BirthDate)).ShouldBe(
        [
            (ApiRequests.ClientFullName, "ana@example.com", (DateOnly?)null),
            (ChildFullName, null, new DateOnly(2014, 3, 14)),
        ]);
        students.Single(student => student.FullName == ApiRequests.ClientFullName).UserId.ShouldBe(client.UserId);
    }
}
