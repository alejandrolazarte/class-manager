namespace ClassManager.Api.I.Tests.Endpoints.Sessions.When_getting_a_session_of_an_earlier_day;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_is_past(ApiFixture fixture)
{
    [Fact]
    public async Task Then_it_is_past_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var classGroup = await business.HttpClient.CreateClassGroupWithInstructorAsync();
        var lastWeek = EnrollmentRequests.Today.AddDays(-7);

        var session = await business.HttpClient.GetSessionAsync(classGroup.Id, lastWeek);

        session!.IsPast.ShouldBeTrue();
    }
}
