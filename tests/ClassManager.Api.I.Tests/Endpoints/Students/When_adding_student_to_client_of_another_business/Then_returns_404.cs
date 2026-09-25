namespace ClassManager.Api.I.Tests.Endpoints.Students.When_adding_student_to_client_of_another_business;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_404(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_404_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var otherBusiness = await fixture.SeedBusinessAsync();
        var otherBusinessClient = await otherBusiness.HttpClient.RegisterClientAsync();

        using var response = await business.HttpClient.PostStudentAsync(otherBusinessClient.Id);

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}
