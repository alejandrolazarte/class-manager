using ClassManager.Core.UseCases.Students;

namespace ClassManager.Api.I.Tests.Endpoints.Students.When_searching_students_by_client_name;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_their_students_are_returned(ApiFixture fixture)
{
    [Fact]
    public async Task Then_their_students_are_returned_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var client = await business.HttpClient.RegisterClientAsync(
            fullName: "Ana Pérez",
            students: [new NewStudent("Tomás Pérez", null, null), new NewStudent("Lucía Pérez", null, null)]);
        await business.HttpClient.RegisterClientAsync(fullName: "Carla Gómez", phoneNumber: "11 5555-6666");

        var students = await business.HttpClient.GetFromJsonAsync<List<StudentSummaryResponse>>(
            new Uri($"{ApiRoutes.Students}?search=ana", UriKind.Relative), ApiRequests.JsonOptions);

        students!.Select(student => student.FullName).ShouldBe(["Lucía Pérez", "Tomás Pérez"]);
        students!.ShouldAllBe(student => student.ClientId == client.Id && student.ClientFullName == "Ana Pérez");
    }
}
