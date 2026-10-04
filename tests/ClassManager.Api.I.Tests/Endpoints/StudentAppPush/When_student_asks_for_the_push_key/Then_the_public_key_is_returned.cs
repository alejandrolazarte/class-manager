namespace ClassManager.Api.I.Tests.Endpoints.StudentAppPush.When_student_asks_for_the_push_key;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_public_key_is_returned(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_public_key_is_returned_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();

        var key = await scenario.Student.GetPushKeyAsync();

        key!.PublicKey.ShouldBe(BusinessApiFactory.VapidPublicKey);
    }
}
