using ClassManager.Core.Domain.Subscriptions;

namespace ClassManager.Api.I.Tests.Endpoints.ClassMaterial.When_free_plan_uploads_a_class_material_pdf;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_is_accepted(ApiFixture fixture)
{
    [Fact]
    public async Task Then_it_is_accepted_Run()
    {
        var business = await fixture.SeedBusinessAsync(PlanCodes.Free);
        var instructor = await business.HttpClient.CreateInstructorAsync();
        var classGroup = await business.HttpClient.CreateClassGroupAsync(ClassGroupRequests.ClassGroupFor(instructor.Id));

        using var response = await business.HttpClient.PostClassMaterialFileAsync(classGroup.Id, ClassMaterialRequests.Pdf);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }
}
