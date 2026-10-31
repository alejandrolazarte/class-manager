using ClassManager.Core.UseCases.Clients;
using ClassManager.Core.UseCases.Students;

namespace ClassManager.Api.I.Tests.Endpoints.Clients.When_a_family_is_registered_with_a_monthly_plan;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_every_student_holds_it(ApiFixture fixture)
{
    [Fact]
    public async Task Then_every_student_holds_it_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var plan = await business.HttpClient.CreateMonthlyPlanAsync(price: 20000m);
        var command = new RegisterClientCommand(
            ApiRequests.ClientFullName,
            ApiRequests.ClientPhoneNumber,
            null,
            null,
            [new NewStudent("Tomás Pérez", null, null), new NewStudent("Lucía Pérez", null, null)],
            PlanId: plan.Id);

        using var response = await business.HttpClient.PostAsJsonAsync(new Uri(ApiRoutes.Clients, UriKind.Relative), command, ApiRequests.JsonOptions);

        response.EnsureSuccessStatusCode();
        var client = await business.HttpClient.GetFromJsonAsync<ClientDetailsResponse>(new Uri($"{ApiRoutes.Clients}/{(await response.Content.ReadFromJsonAsync<ClientDetailsResponse>(ApiRequests.JsonOptions))!.Id}", UriKind.Relative), ApiRequests.JsonOptions);
        client!.Plans.Students.Select(student => student.ClassPackId).ShouldBe([plan.Id, plan.Id]);
        client.Plans.MonthlyDue.ShouldBe(40000m);
    }
}
