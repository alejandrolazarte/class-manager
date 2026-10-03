using ClassManager.Core.Domain.Subscriptions;

namespace ClassManager.Api.I.Tests.Endpoints.Subscriptions.When_the_brand_is_on_enterprise;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_students_are_unlimited_and_counted(ApiFixture fixture)
{
    private const int SeededStudents = 3;

    [Fact]
    public async Task Then_students_are_unlimited_and_counted_Run()
    {
        var business = await fixture.SeedBusinessAsync(PlanCodes.Enterprise);
        await fixture.SeedStudentsAsync(business, SeededStudents);

        var member = await business.HttpClient.GetCurrentMemberAsync();

        var students = member.Subscription!.Features.Single(feature => feature.Code == Features.Students);
        students.Limit.ShouldBeNull();
        students.Used.ShouldBe(SeededStudents);
    }
}
