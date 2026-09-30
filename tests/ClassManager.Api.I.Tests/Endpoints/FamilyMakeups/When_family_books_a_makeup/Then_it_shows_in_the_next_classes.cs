namespace ClassManager.Api.I.Tests.Endpoints.FamilyMakeups.When_family_books_a_makeup;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_shows_in_the_next_classes(ApiFixture fixture)
{
    [Fact]
    public async Task Then_it_shows_in_the_next_classes_Run()
    {
        var scenario = await fixture.SeedFamilyScenarioAsync();
        await scenario.NoticeAndBookOtherClassAsync();

        var home = await scenario.Family.GetFamilyHomeAsync();

        home!.Students.Single().NextClasses
            .ShouldContain(nextClass => nextClass.ClassGroupId == scenario.Coaches.OtherClassGroup.Id && nextClass.IsMakeup);
    }
}
