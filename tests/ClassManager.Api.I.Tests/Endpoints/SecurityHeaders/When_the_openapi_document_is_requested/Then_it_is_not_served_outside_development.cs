using System.Net.Http.Headers;

namespace ClassManager.Api.I.Tests.Endpoints.SecurityHeaders.When_the_openapi_document_is_requested;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_is_not_served_outside_development(ApiFixture fixture)
{
    private const string OpenApiDocumentPath = "/openapi/v1.json";
    private const string EnvironmentSettingKey = "environment";
    private const string ProductionEnvironment = "Production";

    [Fact]
    public async Task Then_it_is_not_served_outside_development_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var accessToken = fixture.CreateAccessToken(business.Business.Id, userId: business.OwnerUserId);
        using var productionFactory = fixture.ApiFactory
            .WithWebHostBuilder(builder => builder.UseSetting(EnvironmentSettingKey, ProductionEnvironment));
        using var client = productionFactory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(ApiFixture.BearerScheme, accessToken);

        using var response = await client.GetAsync(new Uri(OpenApiDocumentPath, UriKind.Relative));

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}
