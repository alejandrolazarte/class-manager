namespace ClassManager.Api.I.Tests.Endpoints.Families.When_two_families_exist;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_each_sees_only_their_own_students(ApiFixture fixture)
{
    [Fact]
    public async Task Then_each_sees_only_their_own_students_Run()
    {
        var coaches = await fixture.SeedCoachScenarioAsync();
        var firstFamily = await fixture.InviteFamilyOfAsync(coaches, CoachScenario.CoachStudentFullName);
        var secondFamily = await fixture.InviteFamilyOfAsync(coaches, CoachScenario.OtherStudentFullName);

        var firstHome = await firstFamily.Family.GetFamilyHomeAsync();
        var secondHome = await secondFamily.Family.GetFamilyHomeAsync();

        firstHome!.Students.Select(student => student.FullName).ShouldBe([CoachScenario.CoachStudentFullName]);
        secondHome!.Students.Select(student => student.FullName).ShouldBe([CoachScenario.OtherStudentFullName]);
    }
}
