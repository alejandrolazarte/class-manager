using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Backoffice.Api.I.Tests.Operators.When_a_reader_lists_the_operators;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_is_forbidden(BackofficeApiFixture fixture)
{
    [Fact]
    public async Task Then_it_is_forbidden_Run()
    {
        using var client = fixture.ApiFactory.CreateHttpsClient();
        var accessToken = await client.OpenSessionAsync(await fixture.SeedOperatorAsync(BusinessRole.PlatformReader));

        using var response = await client.GetAsync(BackofficeRequests.OperatorsRoute, accessToken);

        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }
}
