using ClassManager.Core.UseCases.Clients;
using ClassManager.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ClassManager.Api.I.Tests.Endpoints.Clients.When_a_client_who_is_also_an_instructor_is_separated;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_app_access_stays_with_the_instructor(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_app_access_stays_with_the_instructor_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        var owner = scenario.Instructors.Business.HttpClient;
        var businessId = scenario.Instructors.Business.Business.Id;
        var (_, sharedUserId) = await fixture.SeedInstructorWhoIsAStudentAsync(scenario);
        var client = await owner.GetFromJsonAsync<ClientDetailsResponse>(new Uri($"{ApiRoutes.Clients}/{scenario.ClientId}", UriKind.Relative), ApiRequests.JsonOptions);
        client!.IsOnTheTeam.ShouldBeTrue();

        using var response = await owner.PostSeparationAsync($"{ApiRoutes.Clients}/{scenario.ClientId}", "Lucía Gómez");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        await using var context = fixture.CreateDbContext(businessId);
        var separated = await context.Clients.AsNoTracking().Include(row => row.User).SingleAsync(row => row.Id == scenario.ClientId);
        separated.UserId.ShouldNotBe(sharedUserId);
        separated.User.PhoneNumber.ShouldNotBeNull();
        (await context.ClientAccounts.AnyAsync(account => account.ClientId == scenario.ClientId)).ShouldBeFalse();
        (await context.ClientAccounts
            .IgnoreQueryFilters([SoftDeleteModelBuilderExtensions.SoftDeleteQueryFilter])
            .AnyAsync(account => account.ClientId == scenario.ClientId && account.UserId == sharedUserId)).ShouldBeTrue();
    }
}
