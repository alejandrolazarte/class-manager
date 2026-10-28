namespace ClassManager.Api.I.Tests.Endpoints.ClassGroups.When_reading_the_week_of_an_instructor_of_another_business;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_404(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_404_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var otherBusiness = await fixture.SeedBusinessAsync();
        var classGroup = await business.HttpClient.CreateClassGroupWithInstructorAsync();

        using var response = await otherBusiness.HttpClient.GetInstructorWeekAsync(classGroup.InstructorId);

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}
