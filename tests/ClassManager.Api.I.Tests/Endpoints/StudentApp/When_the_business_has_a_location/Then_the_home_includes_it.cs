using ClassManager.Core.UseCases.StudentApp;

namespace ClassManager.Api.I.Tests.Endpoints.StudentApp.When_the_business_has_a_location;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_home_includes_it(ApiFixture fixture)
{
    private const string LocationName = "Av. Costanera 1450, Ciudad";
    private const string LocationMapsUrl = "https://maps.app.goo.gl/k3Hq8vPzR2";

    [Fact]
    public async Task Then_the_home_includes_it_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        using (var response = await business.HttpClient.PutBusinessAsync(
            SettingsRequests.SettingsOf(business, locationName: LocationName, locationMapsUrl: LocationMapsUrl)))
        {
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
        }

        var clientEmail = StudentAppRequests.UniqueStudentEmail();
        var family = await business.HttpClient.RegisterFamilyAsync(clientEmail);
        using var clientApp = await fixture.SignInClientOfFamilyAsync(business.HttpClient, family.Id, clientEmail);

        var home = await clientApp.GetStudentAppHomeAsync();

        home!.Location.ShouldBe(new StudentAppLocationResponse(LocationName, LocationMapsUrl));
    }
}
