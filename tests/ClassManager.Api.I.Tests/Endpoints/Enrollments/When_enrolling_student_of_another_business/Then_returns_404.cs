namespace ClassManager.Api.I.Tests.Endpoints.Enrollments.When_enrolling_student_of_another_business;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_404(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_404_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var otherBusiness = await fixture.SeedBusinessAsync();
        var classGroup = await business.HttpClient.CreateClassGroupWithInstructorAsync();
        var otherStudentId = await otherBusiness.HttpClient.RegisterStudentAsync();

        using var response = await business.HttpClient.PostEnrollmentAsync(classGroup.Id, otherStudentId);

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}
