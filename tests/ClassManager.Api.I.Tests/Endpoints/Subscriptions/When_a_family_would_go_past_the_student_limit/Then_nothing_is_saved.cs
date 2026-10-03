using ClassManager.Core.Domain.Subscriptions;
using ClassManager.Core.UseCases.Students;
using Microsoft.EntityFrameworkCore;

namespace ClassManager.Api.I.Tests.Endpoints.Subscriptions.When_a_family_would_go_past_the_student_limit;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_nothing_is_saved(ApiFixture fixture)
{
    private const int OneBelowFreeStudentLimit = 29;

    [Fact]
    public async Task Then_nothing_is_saved_Run()
    {
        var business = await fixture.SeedBusinessAsync(PlanCodes.Free);
        await fixture.SeedStudentsAsync(business, OneBelowFreeStudentLimit);

        using var response = await business.HttpClient.PostClientAsync(
            fullName: "María Gómez",
            students: [new NewStudent("Lucas Gómez", null, null), new NewStudent("Sofía Gómez", null, null)]);

        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
        await using var context = fixture.CreateDbContext(business.Business.Id);
        (await context.Students.CountAsync()).ShouldBe(OneBelowFreeStudentLimit);
        (await context.Clients.AnyAsync(client => client.FullName == "María Gómez")).ShouldBeFalse();
    }
}
