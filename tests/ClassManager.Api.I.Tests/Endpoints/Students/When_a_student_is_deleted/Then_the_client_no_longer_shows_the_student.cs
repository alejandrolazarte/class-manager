namespace ClassManager.Api.I.Tests.Endpoints.Students.When_a_student_is_deleted;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_client_no_longer_shows_the_student(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_client_no_longer_shows_the_student_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var owner = business.HttpClient;
        var client = await owner.RegisterClientAsync();
        var student = await owner.AddStudentAsync(client.Id, "Martina Pérez");

        using var response = await owner.DeleteStudentAsync(student.Id);

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        (await owner.GetClientAsync(client.Id)).Students.Select(row => row.Id).ShouldNotContain(student.Id);
    }
}
