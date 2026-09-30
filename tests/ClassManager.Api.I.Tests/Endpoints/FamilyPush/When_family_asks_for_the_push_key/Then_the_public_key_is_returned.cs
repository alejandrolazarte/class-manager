namespace ClassManager.Api.I.Tests.Endpoints.FamilyPush.When_family_asks_for_the_push_key;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_public_key_is_returned(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_public_key_is_returned_Run()
    {
        var scenario = await fixture.SeedFamilyScenarioAsync();

        var key = await scenario.Family.GetPushKeyAsync();

        key!.PublicKey.ShouldBe(BusinessApiFactory.VapidPublicKey);
    }
}
