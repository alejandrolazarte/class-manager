namespace ClassManager.Api.I.Tests.Endpoints.FamilyPackClasses.When_family_cancels_a_pack_class;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_class_is_given_back(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_class_is_given_back_Run()
    {
        var scenario = await fixture.SeedPackFamilyScenarioAsync();
        await scenario.BookPackClassAsync();

        using var response = await scenario.Family.DeletePackClassAsync(scenario.StudentId, scenario.PackClassGroupId, CoachScenario.ClassDate);

        response.EnsureSuccessStatusCode();
        (await scenario.Family.GetPackClassesAsync(scenario.StudentId))!.ClassesLeft.ShouldBe(4);
    }
}
