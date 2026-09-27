using ClassManager.Core.Domain.Fees;
using ClassManager.Core.Domain.Sessions;
using ClassManager.Core.UseCases.Students;

namespace ClassManager.Api.I.Tests.Endpoints.PrivateLessons.When_trial_student_attends;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_no_pack_class_is_used(ApiFixture fixture)
{
    [Fact]
    public async Task Then_no_pack_class_is_used_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var instructor = await business.HttpClient.CreateInstructorAsync();
        var client = await business.HttpClient.RegisterClientAsync(students: [new NewStudent(ApiRequests.StudentFullName, null, null)]);
        (await business.HttpClient.PutBillingPlanAsync(client.Id, BillingPlanKind.ClassPacks)).EnsureSuccessStatusCode();
        var pack = await business.HttpClient.CreateClassPackAsync();
        await business.HttpClient.SellClassPackAsync(client.Id, pack.Id);
        var trial = await business.HttpClient.SchedulePrivateLessonAsync(
            instructor.Id, client.Students[0].Id, EnrollmentRequests.Today, trialPrice: 0m);

        (await business.HttpClient.PutPrivateLessonAttendanceAsync(trial.Id, client.Students[0].Id, AttendanceStatus.Present)).EnsureSuccessStatusCode();

        var balance = await business.HttpClient.GetClassBalanceAsync(client.Id);
        balance!.Purchases.Single().UsedClasses.ShouldBe(0);
    }
}
