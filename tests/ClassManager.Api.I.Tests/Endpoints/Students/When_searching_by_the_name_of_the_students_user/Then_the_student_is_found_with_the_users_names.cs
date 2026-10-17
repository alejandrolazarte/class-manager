using ClassManager.Core.UseCases.Students;

namespace ClassManager.Api.I.Tests.Endpoints.Students.When_searching_by_the_name_of_the_students_user;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_student_is_found_with_the_users_names(ApiFixture fixture)
{
    private const string StudentUserFullName = "Tomás Andrés Pérez";
    private const string ContactUserFullName = "Ana María Pérez";

    [Fact]
    public async Task Then_the_student_is_found_with_the_users_names_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var client = await business.HttpClient.RegisterClientAsync(students: [new NewStudent(ApiRequests.StudentFullName, null, null)]);
        await fixture.ChangeUserOfClientAsync(business.Business.Id, client.Id, ContactUserFullName);
        await fixture.ChangeUserOfStudentAsync(business.Business.Id, client.Students.Single().Id, StudentUserFullName);

        var students = await business.HttpClient.SearchStudentsAsync("Andrés");

        students!.Select(student => (student.FullName, student.ClientFullName)).ShouldBe([(StudentUserFullName, ContactUserFullName)]);
    }
}
