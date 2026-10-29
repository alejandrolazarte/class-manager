namespace ClassManager.Api.I.Tests.Endpoints.UserPhotos.When_another_family_approves_the_photo;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_is_not_found(ApiFixture fixture)
{
    private const string OtherFamilyPhoneNumber = "11 9876-5432";

    [Fact]
    public async Task Then_it_is_not_found_Run()
    {
        using var scenario = await fixture.SeedGuardedChildAsync();
        await scenario.Child.ChangeMyPhotoAsync();
        var approval = (await scenario.Guardian.ListPhotoApprovalsAsync()).Single();
        var otherClientEmail = StudentAppRequests.UniqueStudentEmail();
        var otherFamily = await scenario.Business.HttpClient.RegisterFamilyAsync(otherClientEmail, OtherFamilyPhoneNumber);
        using var otherGuardian = await fixture.SignInClientOfFamilyAsync(scenario.Business.HttpClient, otherFamily.Id, otherClientEmail);

        using var response = await otherGuardian.PostPhotoApprovalAsync(approval.Id);

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}
