namespace ClassManager.Api.I.Tests.Endpoints.Enrollments.When_enrolling_in_full_class;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_409(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_409_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var classGroup = await business.HttpClient.CreateClassGroupWithInstructorAsync(capacity: 1);
        await business.HttpClient.EnrollAsync(classGroup.Id, await business.HttpClient.RegisterStudentAsync("Tomás Pérez", "11 1111-1111"));
        var secondStudentId = await business.HttpClient.RegisterStudentAsync("Lucía Gómez", "11 2222-2222");

        using var response = await business.HttpClient.PostEnrollmentAsync(classGroup.Id, secondStudentId);

        response.StatusCode.ShouldBe(HttpStatusCode.Conflict);
    }
}
