namespace ClassManager.Api.I.Tests.Endpoints.ClassPacks.When_creating_a_pack_for_a_class_of_another_business;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_400(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_400_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var otherBusiness = await fixture.SeedBusinessAsync();
        var otherClassGroup = await otherBusiness.HttpClient.CreateClassGroupWithInstructorAsync();

        using var response = await business.HttpClient.PostClassPackAsync(classGroupIds: [otherClassGroup.Id]);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }
}
