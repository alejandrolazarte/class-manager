using ClassManager.Core.Domain.Enrollments;
using ClassManager.Core.Domain.Fees;
using ClassManager.Core.UseCases.Students;

namespace ClassManager.Api.I.Tests.Endpoints.Enrollments.When_enrolling_outside_the_classes_of_the_pack;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_409(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_409_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var packClassGroup = await business.HttpClient.CreateClassGroupWithInstructorAsync();
        var otherClassGroup = await business.HttpClient.CreateClassGroupAsync(
            ClassGroupRequests.ClassGroupFor(packClassGroup.InstructorId, startTime: "20:00") with { Name = "Aquagym" });
        var client = await business.HttpClient.RegisterClientAsync(students: [new NewStudent(ApiRequests.StudentFullName, null, null)]);
        (await business.HttpClient.PutBillingPlanAsync(client.Id, BillingPlanKind.ClassPacks)).EnsureSuccessStatusCode();
        var classPack = await business.HttpClient.CreateClassPackAsync(classGroupIds: [packClassGroup.Id]);
        await business.HttpClient.SellClassPackAsync(client.Id, classPack.Id);

        using var response = await business.HttpClient.PostEnrollmentAsync(otherClassGroup.Id, client.Students[0].Id);

        response.StatusCode.ShouldBe(HttpStatusCode.Conflict);
        (await response.Content.ReadAsStringAsync()).ShouldContain(EnrollmentErrorCodes.OutsidePack);
    }
}
