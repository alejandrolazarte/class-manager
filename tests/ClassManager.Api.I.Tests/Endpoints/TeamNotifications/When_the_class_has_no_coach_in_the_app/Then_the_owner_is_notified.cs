namespace ClassManager.Api.I.Tests.Endpoints.TeamNotifications.When_the_class_has_no_coach_in_the_app;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_owner_is_notified(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_owner_is_notified_Run()
    {
        var scenario = await fixture.SeedFamilyScenarioAsync();

        await scenario.NoticeAndBookOtherClassAsync();

        var ownerNotifications = await scenario.Coaches.Business.HttpClient.GetTeamNotificationsAsync();
        ownerNotifications!.Items.Single().Title.ShouldBe("Tomás viene a recuperar");
    }
}
