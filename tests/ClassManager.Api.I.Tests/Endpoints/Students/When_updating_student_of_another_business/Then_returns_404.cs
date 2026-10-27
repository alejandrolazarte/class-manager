using ClassManager.Core.UseCases.Students;

namespace ClassManager.Api.I.Tests.Endpoints.Students.When_updating_student_of_another_business;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_404(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_404_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var otherBusiness = await fixture.SeedBusinessAsync();
        var otherBusinessStudentId = await otherBusiness.HttpClient.RegisterStudentAsync();

        using var response = await business.HttpClient.PutStudentAsync(otherBusinessStudentId, new UpdateStudentRequest("Martina Pérez", null, null, null));

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}
