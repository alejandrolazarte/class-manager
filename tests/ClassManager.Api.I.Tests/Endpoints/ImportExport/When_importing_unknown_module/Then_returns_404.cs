
namespace ClassManager.Api.I.Tests.Endpoints.ImportExport.When_importing_unknown_module;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_404(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_404_Run()
    {
        var business = await fixture.SeedBusinessAsync();

        using var response = await business.HttpClient.PostImportFileAsync("payments", ApiRoutes.ImportAction, "Pago\n10\n");

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}
