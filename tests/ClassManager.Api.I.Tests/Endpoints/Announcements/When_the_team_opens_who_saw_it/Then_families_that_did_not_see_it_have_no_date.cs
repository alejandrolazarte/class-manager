namespace ClassManager.Api.I.Tests.Endpoints.Announcements.When_the_team_opens_who_saw_it;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_families_that_did_not_see_it_have_no_date(ApiFixture fixture)
{
    [Fact]
    public async Task Then_families_that_did_not_see_it_have_no_date_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        var announcement = await scenario.Instructors.Business.HttpClient.PublishAnnouncementAsync();

        var recipients = await scenario.Instructors.Business.HttpClient.GetAnnouncementRecipientsAsync(announcement.Id);

        var recipient = recipients!.Single();
        recipient.ClientId.ShouldBe(scenario.ClientId);
        recipient.SeenAt.ShouldBeNull();
    }
}
