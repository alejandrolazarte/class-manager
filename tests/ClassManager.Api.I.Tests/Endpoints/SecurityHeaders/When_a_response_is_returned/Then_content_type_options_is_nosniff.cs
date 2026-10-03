using ClassManager.Api.SecurityHeaders;

namespace ClassManager.Api.I.Tests.Endpoints.SecurityHeaders.When_a_response_is_returned;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_content_type_options_is_nosniff(ApiFixture fixture)
{
    [Fact]
    public async Task Then_content_type_options_is_nosniff_Run()
    {
        using var client = fixture.ApiFactory.CreateClient();

        using var response = await client.GetAsync(new Uri(ApiRoutes.Health, UriKind.Relative));

        response.Headers.GetValues(SecurityHeadersMiddleware.ContentTypeOptionsHeader)
            .ShouldBe([SecurityHeadersMiddleware.ContentTypeOptionsValue]);
    }
}
