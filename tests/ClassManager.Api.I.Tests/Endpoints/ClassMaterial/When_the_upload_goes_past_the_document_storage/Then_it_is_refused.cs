using ClassManager.Core.Domain.Subscriptions;
using ClassManager.Subscriptions.Access;

namespace ClassManager.Api.I.Tests.Endpoints.ClassMaterial.When_the_upload_goes_past_the_document_storage;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_is_refused(ApiFixture fixture)
{
    private const int NoDocumentStorage = 0;

    [Fact]
    public async Task Then_it_is_refused_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        await fixture.AddFeatureAsync(business, Features.DocumentStorage, NoDocumentStorage);
        var instructor = await business.HttpClient.CreateInstructorAsync();
        var classGroup = await business.HttpClient.CreateClassGroupAsync(ClassGroupRequests.ClassGroupFor(instructor.Id));

        using var response = await business.HttpClient.PostClassMaterialFileAsync(classGroup.Id, ClassMaterialRequests.Pdf);

        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
        (await response.Content.ReadAsStringAsync()).ShouldContain(FeatureErrorCodes.LimitReached);
    }
}
