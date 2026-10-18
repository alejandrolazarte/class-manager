using ClassManager.Core.UseCases.Clients;
using Microsoft.EntityFrameworkCore;

namespace ClassManager.Api.I.Tests.Endpoints.Clients.When_a_client_is_registered_as_the_same_person_as_an_instructor;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_they_keep_one_user(ApiFixture fixture)
{
    [Fact]
    public async Task Then_they_keep_one_user_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var owner = business.HttpClient;
        var email = MemberRequests.UniqueInviteeEmail();
        var instructor = await owner.CreateInstructorAsync("Ana Gómez", email);
        var instructorUserId = await fixture.UserIdOfInstructorAsync(business.Business.Id, instructor.Id);

        using var response = await owner.PostClientWithEmailAsync("Ana Gómez", email, instructorUserId);

        response.StatusCode.ShouldBe(HttpStatusCode.Created);
        var client = (await response.Content.ReadFromJsonAsync<ClientDetailsResponse>(ApiRequests.JsonOptions))!;
        await using var context = fixture.CreateDbContext(business.Business.Id);
        var registered = await context.Clients.AsNoTracking().Include(row => row.User).SingleAsync(row => row.Id == client.Id);
        registered.UserId.ShouldBe(instructorUserId);
        registered.User.PhoneNumber.ShouldNotBeNull();
    }
}
