using ClassManager.Core.Domain.Subscriptions;

namespace ClassManager.Api.I.Tests.Endpoints.Subscriptions.When_the_business_has_uploaded_files;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_document_storage_used_is_reported(ApiFixture fixture)
{
    private const int StartedMegabyte = 1;

    [Fact]
    public async Task Then_the_document_storage_used_is_reported_Run()
    {
        var business = await fixture.SeedBusinessAsync(PlanCodes.Pro);
        var instructor = await business.HttpClient.CreateInstructorAsync();
        var classGroup = await business.HttpClient.CreateClassGroupAsync(ClassGroupRequests.ClassGroupFor(instructor.Id));
        await business.HttpClient.UploadClassMaterialFileAsync(classGroup.Id);

        var member = await business.HttpClient.GetCurrentMemberAsync();

        member.Subscription!.Features.Single(feature => feature.Code == Features.DocumentStorage).Used.ShouldBe(StartedMegabyte);
    }
}
