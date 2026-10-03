namespace ClassManager.Api.I.Tests.Endpoints.FamilyPackClasses.When_family_books_a_pack_class;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_spot_is_taken(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_spot_is_taken_Run()
    {
        var scenario = await fixture.SeedPackFamilyScenarioAsync();

        await scenario.BookPackClassAsync();

        var makeups = await scenario.Family.GetMakeupsAsync(scenario.StudentId);
        makeups!.Slots.Single(slot => slot.ClassGroupId == scenario.PackClassGroupId && slot.Date == CoachScenario.ClassDate).SpotsLeft.ShouldBe(6);
    }
}
