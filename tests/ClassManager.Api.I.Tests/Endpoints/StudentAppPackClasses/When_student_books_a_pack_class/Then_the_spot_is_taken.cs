namespace ClassManager.Api.I.Tests.Endpoints.StudentAppPackClasses.When_student_books_a_pack_class;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_spot_is_taken(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_spot_is_taken_Run()
    {
        var scenario = await fixture.SeedPackStudentAppScenarioAsync();

        await scenario.BookPackClassAsync();

        var makeups = await scenario.Student.GetMakeupsAsync(scenario.StudentId);
        makeups!.Slots.Single(slot => slot.ClassGroupId == scenario.PackClassGroupId && slot.Date == CoachScenario.ClassDate).SpotsLeft.ShouldBe(6);
    }
}
