namespace ClassManager.Api.I.Tests.Endpoints.ClassPacks.When_a_pack_is_created_for_any_group_class;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_keeps_that_option_and_no_class_list(ApiFixture fixture)
{
    [Fact]
    public async Task Then_it_keeps_that_option_and_no_class_list_Run()
    {
        var business = await fixture.SeedBusinessAsync();

        var pack = await business.HttpClient.CreateAnyGroupClassPackAsync();

        pack.ShouldSatisfyAllConditions(
            savedPack => savedPack.CoversAnyGroupClass.ShouldBeTrue(),
            savedPack => savedPack.ClassGroupIds.ShouldBeEmpty(),
            savedPack => savedPack.ClassDurationMinutes.ShouldBeNull());
    }
}
