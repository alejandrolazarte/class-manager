namespace ClassManager.Api.I.Tests.Endpoints.FamilyMakeups.When_family_books_a_makeup;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_day_list_counts_the_student(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_day_list_counts_the_student_Run()
    {
        var scenario = await fixture.SeedFamilyScenarioAsync();
        await scenario.NoticeAndBookOtherClassAsync();

        var day = await scenario.Coaches.Business.HttpClient.GetDayAsync(CoachScenario.ClassDate);

        day!.Single(session => session.ClassGroupId == scenario.Coaches.OtherClassGroup.Id).EnrolledCount.ShouldBe(2);
    }
}
