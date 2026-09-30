namespace ClassManager.Api.I.Tests.Endpoints.FamilyPush.When_school_cancels_a_class;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_its_families_get_a_push(ApiFixture fixture)
{
    [Fact]
    public async Task Then_its_families_get_a_push_Run()
    {
        var scenario = await fixture.SeedFamilyScenarioAsync();
        var endpoint = await scenario.SubscribeAsync();

        (await scenario.Coaches.Business.HttpClient.PutSessionCancellationAsync(
            scenario.Coaches.CoachClassGroup.Id, CoachScenario.ClassDate, "Feriado")).EnsureSuccessStatusCode();

        var push = await fixture.ApiFactory.PushSender.WaitForPushToAsync(endpoint);
        push.Title.ShouldBe($"Se suspende {CoachScenario.CoachClassGroupName}");
        push.Body.ShouldBe("El jueves 24/9 a las 18:00 no hay clase. Feriado");
    }
}
