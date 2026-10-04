namespace ClassManager.Api.I.Tests.Endpoints.StudentAppPush.When_student_sends_a_subscription_that_is_not_https;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_400(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_400_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();

        using var response = await scenario.Student.PutPushSubscriptionAsync(
            PushRequests.NewSubscription() with { Endpoint = "http://push.example.com/send/1" });

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }
}
