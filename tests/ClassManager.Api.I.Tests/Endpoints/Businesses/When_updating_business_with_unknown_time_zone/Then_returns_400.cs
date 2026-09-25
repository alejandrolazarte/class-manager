using System.Text.Json;

using ClassManager.Core.UseCases.Businesses;

namespace ClassManager.Api.I.Tests.Endpoints.Businesses.When_updating_business_with_unknown_time_zone;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_400(ApiFixture fixture)
{
    private const string UnknownTimeZoneId = "Mars/Olympus_Mons";
    private const string ErrorsProperty = "errors";

    [Fact]
    public async Task Then_returns_400_Run()
    {
        var business = await fixture.SeedBusinessAsync();

        using var response = await business.HttpClient.PutBusinessAsync(SettingsRequests.SettingsOf(business, UnknownTimeZoneId));

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        var problem = await response.ReadProblemAsync();
        problem.GetProperty(ErrorsProperty)
            .TryGetProperty(JsonNamingPolicy.CamelCase.ConvertName(nameof(UpdateBusinessSettingsCommand.TimeZoneId)), out _)
            .ShouldBeTrue();
    }
}
