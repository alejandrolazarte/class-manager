using ClassManager.Core.UseCases.Students;

namespace ClassManager.Api.I.Tests.Endpoints.Enrollments.When_enrolling_a_student_on_a_monthly_fee_outside_their_pack;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_student_is_enrolled(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_student_is_enrolled_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var packClassGroup = await business.HttpClient.CreateClassGroupWithInstructorAsync();
        var otherClassGroup = await business.HttpClient.CreateClassGroupAsync(
            ClassGroupRequests.ClassGroupFor(packClassGroup.InstructorId, startTime: "20:00") with { Name = "Aquagym" });
        var client = await business.HttpClient.RegisterClientAsync(students: [new NewStudent(ApiRequests.StudentFullName, null, null)]);
        var classPack = await business.HttpClient.CreateClassPackAsync(classGroupIds: [packClassGroup.Id]);
        await business.HttpClient.SellClassPackAsync(client.Id, classPack.Id);

        using var response = await business.HttpClient.PostEnrollmentAsync(otherClassGroup.Id, client.Students[0].Id);

        response.StatusCode.ShouldBe(HttpStatusCode.Created);
    }
}
