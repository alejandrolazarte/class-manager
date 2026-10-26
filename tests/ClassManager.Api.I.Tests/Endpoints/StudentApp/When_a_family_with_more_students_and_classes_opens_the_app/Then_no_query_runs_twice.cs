using ClassManager.Core.UseCases.Students;

namespace ClassManager.Api.I.Tests.Endpoints.StudentApp.When_a_family_with_more_students_and_classes_opens_the_app;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_no_query_runs_twice(ApiFixture fixture)
{
    [Fact]
    public async Task Then_no_query_runs_twice_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var owner = business.HttpClient;
        var firstClass = await owner.CreateClassGroupWithInstructorAsync();
        var secondClass = await owner.CreateClassGroupAsync(
            ClassGroupRequests.ClassGroupFor(firstClass.InstructorId, [DayOfWeek.Monday, DayOfWeek.Wednesday]));
        var family = await owner.RegisterClientAsync(
            "Laura Gómez", "11 4000-1002", [new NewStudent("Mateo Gómez", null, null), new NewStudent("Lucía Gómez", null, null)]);
        foreach (var student in family.Students)
        {
            await owner.EnrollAsync(firstClass.Id, student.Id);
            await owner.EnrollAsync(secondClass.Id, student.Id);
        }

        await using var counter = new SqlCommandCounter(fixture.ConnectionString);
        using var familyApp = counter.CreateClientWithToken(
            await fixture.AcceptFamilyInvitationAsync(owner, family.Id, StudentAppRequests.UniqueStudentEmail()));
        await familyApp.GetStudentAppHomeAsync();

        var queries = await counter.CommandTextsAsync(() => familyApp.GetStudentAppHomeAsync());

        queries.ShouldBeUnique();
    }
}
