using ClassManager.Core.UseCases.Students;

namespace ClassManager.Api.I.Tests.Endpoints.StudentApp.When_a_family_with_more_students_and_classes_opens_the_app;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_home_screen_makes_as_many_queries_as_for_a_small_family(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_home_screen_makes_as_many_queries_as_for_a_small_family_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var owner = business.HttpClient;
        var firstClass = await owner.CreateClassGroupWithInstructorAsync();
        var secondClass = await owner.CreateClassGroupAsync(
            ClassGroupRequests.ClassGroupFor(firstClass.InstructorId, [DayOfWeek.Monday, DayOfWeek.Wednesday]));
        var smallFamily = await owner.RegisterClientAsync(phoneNumber: "11 4000-1001", students: [new NewStudent("Tomás Pérez", null, null)]);
        await owner.EnrollAsync(firstClass.Id, smallFamily.Students[0].Id);
        var largeFamily = await owner.RegisterClientAsync(
            "Laura Gómez", "11 4000-1002", [new NewStudent("Mateo Gómez", null, null), new NewStudent("Lucía Gómez", null, null)]);
        foreach (var student in largeFamily.Students)
        {
            await owner.EnrollAsync(firstClass.Id, student.Id);
            await owner.EnrollAsync(secondClass.Id, student.Id);
        }

        await using var counter = new SqlCommandCounter(fixture.ConnectionString);
        using var smallFamilyApp = counter.CreateClientWithToken(
            await fixture.AcceptFamilyInvitationAsync(owner, smallFamily.Id, StudentAppRequests.UniqueStudentEmail()));
        using var largeFamilyApp = counter.CreateClientWithToken(
            await fixture.AcceptFamilyInvitationAsync(owner, largeFamily.Id, StudentAppRequests.UniqueStudentEmail()));
        await smallFamilyApp.GetStudentAppHomeAsync();
        await largeFamilyApp.GetStudentAppHomeAsync();

        var smallFamilyQueries = await counter.CountAsync(() => smallFamilyApp.GetStudentAppHomeAsync());
        var largeFamilyQueries = await counter.CountAsync(() => largeFamilyApp.GetStudentAppHomeAsync());

        largeFamilyQueries.ShouldBe(smallFamilyQueries);
    }
}
