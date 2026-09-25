namespace ClassManager.Api.I.Tests.Endpoints.Sessions.When_getting_session_of_another_business;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_404(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_404_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var otherBusiness = await fixture.SeedBusinessAsync();
        var otherClassGroup = await otherBusiness.HttpClient.CreateClassGroupWithInstructorAsync();

        using var response = await business.HttpClient.GetAsync(
            new Uri(SessionRequests.SessionPath(otherClassGroup.Id, EnrollmentRequests.Today), UriKind.Relative));

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}
