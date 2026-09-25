using ClassManager.Core.UseCases.Students;

namespace ClassManager.Api.I.Tests.Endpoints.Students.When_searching_students;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_only_current_business_students_are_returned(ApiFixture fixture)
{
    [Fact]
    public async Task Then_only_current_business_students_are_returned_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var otherBusiness = await fixture.SeedBusinessAsync();
        var ownClient = await business.HttpClient.RegisterClientAsync(students: [new NewStudent(ApiRequests.StudentFullName, null, null)]);
        await otherBusiness.HttpClient.RegisterClientAsync(students: [new NewStudent(ApiRequests.StudentFullName, null, null)]);

        var students = await business.HttpClient.GetFromJsonAsync<List<StudentSummaryResponse>>(
            new Uri($"{ApiRoutes.Students}?search=tom", UriKind.Relative), ApiRequests.JsonOptions);

        students!.Select(student => student.Id).ShouldBe([ownClient.Students[0].Id]);
    }
}
