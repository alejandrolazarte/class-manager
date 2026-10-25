using ClassManager.Core.UseCases.Students;

namespace ClassManager.Api.I.Tests.Endpoints.Sessions.When_a_student_pays_a_monthly_fee;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_session_shows_no_class_balance(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_session_shows_no_class_balance_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        await business.HttpClient.SetDefaultFeeAsync();
        var classGroup = await business.HttpClient.CreateClassGroupWithInstructorAsync();
        var client = await business.HttpClient.RegisterClientAsync(students: [new NewStudent(ApiRequests.StudentFullName, null, null)]);
        await business.HttpClient.EnrollAsync(classGroup.Id, client.Students[0].Id);

        var session = await business.HttpClient.GetSessionAsync(classGroup.Id, EnrollmentRequests.Today);

        session!.Students.Single().ClassBalance.ShouldBeNull();
    }
}
