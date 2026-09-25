namespace ClassManager.Api.I.Tests.Endpoints.Students.When_getting_student_of_another_business;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_404(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_404_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var otherBusiness = await fixture.SeedBusinessAsync();
        var otherBusinessClient = await otherBusiness.HttpClient.RegisterClientAsync();
        var otherBusinessStudent = await otherBusiness.HttpClient.AddStudentAsync(otherBusinessClient.Id);

        using var response = await business.HttpClient.GetAsync(new Uri($"{ApiRoutes.Students}/{otherBusinessStudent.Id}", UriKind.Relative));

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}
