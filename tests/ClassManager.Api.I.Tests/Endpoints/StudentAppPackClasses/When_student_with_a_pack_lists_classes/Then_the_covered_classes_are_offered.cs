namespace ClassManager.Api.I.Tests.Endpoints.StudentAppPackClasses.When_student_with_a_pack_lists_classes;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_covered_classes_are_offered(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_covered_classes_are_offered_Run()
    {
        var scenario = await fixture.SeedPackStudentAppScenarioAsync();

        var packClasses = await scenario.Student.GetPackClassesAsync(scenario.StudentId);

        packClasses!.Slots.Select(slot => slot.ClassGroupId).Distinct().ShouldBe([scenario.PackClassGroupId]);
        packClasses.Slots[0].SpotsLeft.ShouldBe(7);
        packClasses.ClassesLeft.ShouldBe(4);
    }
}
