using System.Text.Json;

namespace ClassManager.Backoffice.Api.I.Tests.Brands.When_the_price_is_negative;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_price_field_is_refused(BackofficeApiFixture fixture)
{
    [Fact]
    public async Task Then_the_price_field_is_refused_Run()
    {
        var changes = await BrandChanges.OnProAsync(fixture);

        using var response = await changes.ChangePlanAsync("lite", price: -1m);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        using var problem = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        problem.RootElement.GetProperty("errors").TryGetProperty("price", out _).ShouldBeTrue();
    }
}
