using ClassManager.Core.UseCases.Announcements;

namespace ClassManager.Api.I.Tests.Endpoints.Announcements.When_an_announcement_is_edited_for_a_class_of_another_business;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_400(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_400_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var otherBusiness = await fixture.SeedInstructorScenarioAsync();
        var published = await business.HttpClient.PublishAnnouncementAsync();

        using var response = await business.HttpClient.PutAnnouncementAsync(published.Id, new UpdateAnnouncementRequest(
            "Cambia el horario", null, ClassGroupIds: [otherBusiness.InstructorClassGroup.Id]));

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        (await response.Content.ReadAsStringAsync()).ShouldContain(AnnouncementErrorCodes.ClassGroupNotFound);
    }
}
