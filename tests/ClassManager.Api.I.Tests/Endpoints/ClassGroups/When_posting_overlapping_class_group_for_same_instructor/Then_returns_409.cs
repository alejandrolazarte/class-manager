namespace ClassManager.Api.I.Tests.Endpoints.ClassGroups.When_posting_overlapping_class_group_for_same_instructor;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_409(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_409_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var instructor = await business.HttpClient.CreateInstructorAsync();
        await business.HttpClient.CreateClassGroupAsync(ClassGroupRequests.ClassGroupFor(instructor.Id));

        using var response = await business.HttpClient.PostClassGroupAsync(
            ClassGroupRequests.ClassGroupFor(instructor.Id, weekdays: [DayOfWeek.Thursday], startTime: "18:30"));

        response.StatusCode.ShouldBe(HttpStatusCode.Conflict);
    }
}
