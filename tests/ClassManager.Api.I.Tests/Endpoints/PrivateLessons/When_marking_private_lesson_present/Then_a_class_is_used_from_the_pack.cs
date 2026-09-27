using ClassManager.Core.Domain.Fees;
using ClassManager.Core.Domain.Sessions;
using ClassManager.Core.UseCases.Students;

namespace ClassManager.Api.I.Tests.Endpoints.PrivateLessons.When_marking_private_lesson_present;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_a_class_is_used_from_the_pack(ApiFixture fixture)
{
    [Fact]
    public async Task Then_a_class_is_used_from_the_pack_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var instructor = await business.HttpClient.CreateInstructorAsync();
        var client = await business.HttpClient.RegisterClientAsync(students: [new NewStudent(ApiRequests.StudentFullName, null, null)]);
        (await business.HttpClient.PutBillingPlanAsync(client.Id, BillingPlanKind.ClassPacks)).EnsureSuccessStatusCode();
        var pack = await business.HttpClient.CreateClassPackAsync();
        await business.HttpClient.SellClassPackAsync(client.Id, pack.Id);
        var lesson = await business.HttpClient.SchedulePrivateLessonAsync(instructor.Id, client.Students[0].Id, EnrollmentRequests.Today);

        using var response = await business.HttpClient.PutPrivateLessonAttendanceAsync(lesson.Id, client.Students[0].Id, AttendanceStatus.Present);

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        var balance = await business.HttpClient.GetClassBalanceAsync(client.Id);
        balance!.Purchases.Single().UsedClasses.ShouldBe(1);
    }
}
