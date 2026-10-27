namespace ClassManager.Api.I.Tests.Endpoints.PrivateLessons.When_reading_the_agenda_of_an_instructor_of_another_business;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_404(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_404_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var otherBusiness = await fixture.SeedBusinessAsync();
        var classGroup = await business.HttpClient.CreateClassGroupWithInstructorAsync();

        using var response = await otherBusiness.HttpClient.GetInstructorAgendaAsync(classGroup.InstructorId, EnrollmentRequests.Today);

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}
