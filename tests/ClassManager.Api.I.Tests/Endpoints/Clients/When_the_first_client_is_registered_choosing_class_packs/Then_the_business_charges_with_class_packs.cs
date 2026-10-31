using ClassManager.Core.Domain.Fees;
using ClassManager.Core.UseCases.Clients;
using ClassManager.Core.UseCases.Students;

namespace ClassManager.Api.I.Tests.Endpoints.Clients.When_the_first_client_is_registered_choosing_class_packs;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_business_charges_with_class_packs(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_business_charges_with_class_packs_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var command = new RegisterClientCommand(
            ApiRequests.ClientFullName,
            ApiRequests.ClientPhoneNumber,
            null,
            null,
            [new NewStudent(ApiRequests.ClientFullName, null, null)],
            BillingMode: BusinessBillingMode.ClassPacks);

        using var response = await business.HttpClient.PostAsJsonAsync(new Uri(ApiRoutes.Clients, UriKind.Relative), command, ApiRequests.JsonOptions);

        response.EnsureSuccessStatusCode();
        var client = await response.Content.ReadFromJsonAsync<ClientDetailsResponse>(ApiRequests.JsonOptions);
        client!.BillingPlan.Kind.ShouldBe(BillingPlanKind.ClassPacks);
        (await business.HttpClient.GetCurrentBusinessAsync())!.BillingMode.ShouldBe(BusinessBillingMode.ClassPacks);
    }
}
