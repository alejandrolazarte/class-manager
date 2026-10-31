using ClassManager.Core.UseCases.Sessions;
using ClassManager.Core.UseCases.Students;

namespace ClassManager.Api.I.Tests.Endpoints.Sessions.When_a_student_paying_per_class_has_a_pack;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_session_shows_the_classes_left(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_session_shows_the_classes_left_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var classGroup = await business.HttpClient.CreateClassGroupWithInstructorAsync();
        var client = await business.HttpClient.RegisterClientAsync(students: [new NewStudent(ApiRequests.StudentFullName, null, null)]);
        var studentId = client.Students[0].Id;
        await business.HttpClient.EnrollAsync(classGroup.Id, studentId);
        var classPack = await business.HttpClient.CreateClassPackAsync(classCount: 4);
        await business.HttpClient.SellClassPackAsync(client.Id, classPack.Id);

        var session = await business.HttpClient.GetSessionAsync(classGroup.Id, EnrollmentRequests.Today);

        session!.Students.Single().ClassBalance.ShouldBe(new SessionClassBalanceResponse(AvailableClasses: 4, UnpaidClasses: 0));
    }
}
