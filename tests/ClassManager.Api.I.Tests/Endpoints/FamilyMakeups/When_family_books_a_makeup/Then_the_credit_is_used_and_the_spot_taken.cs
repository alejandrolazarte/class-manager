namespace ClassManager.Api.I.Tests.Endpoints.FamilyMakeups.When_family_books_a_makeup;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_credit_is_used_and_the_spot_taken(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_credit_is_used_and_the_spot_taken_Run()
    {
        var scenario = await fixture.SeedFamilyScenarioAsync();
        await scenario.NoticeAndBookOtherClassAsync();

        var makeups = await scenario.Family.GetMakeupsAsync(scenario.Coaches.CoachStudentId);

        makeups!.Credits.ShouldBeEmpty();
        var slot = makeups.Slots.Single(slot => slot.Date == CoachScenario.ClassDate);
        slot.IsBooked.ShouldBeTrue();
        slot.SpotsLeft.ShouldBe(scenario.Coaches.OtherClassGroup.Capacity - 2);
    }
}
