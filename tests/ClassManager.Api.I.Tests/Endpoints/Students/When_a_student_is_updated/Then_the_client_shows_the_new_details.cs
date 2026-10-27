using ClassManager.Core.UseCases.Students;

namespace ClassManager.Api.I.Tests.Endpoints.Students.When_a_student_is_updated;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_client_shows_the_new_details(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_client_shows_the_new_details_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var owner = business.HttpClient;
        var client = await owner.RegisterClientAsync();
        var student = await owner.AddStudentAsync(client.Id, "Martina Peres");

        using var response = await owner.PutStudentAsync(student.Id, new UpdateStudentRequest("Martina Pérez", new DateOnly(2017, 6, 2), "Advanced group", null));

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var updatedStudent = (await owner.GetClientAsync(client.Id)).Students.Single(row => row.Id == student.Id);
        updatedStudent.FullName.ShouldBe("Martina Pérez");
        updatedStudent.BirthDate.ShouldBe(new DateOnly(2017, 6, 2));
    }
}
