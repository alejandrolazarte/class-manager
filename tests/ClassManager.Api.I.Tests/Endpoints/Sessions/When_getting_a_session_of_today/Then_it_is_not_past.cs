namespace ClassManager.Api.I.Tests.Endpoints.Sessions.When_getting_a_session_of_today;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_is_not_past(ApiFixture fixture)
{
    [Fact]
    public async Task Then_it_is_not_past_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var classGroup = await business.HttpClient.CreateClassGroupWithInstructorAsync();

        var session = await business.HttpClient.GetSessionAsync(classGroup.Id, EnrollmentRequests.Today);

        session!.IsPast.ShouldBeFalse();
    }
}
