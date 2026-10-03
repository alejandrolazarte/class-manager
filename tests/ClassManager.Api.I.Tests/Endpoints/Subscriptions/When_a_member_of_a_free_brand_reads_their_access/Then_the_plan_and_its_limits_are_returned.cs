using ClassManager.Core.Domain.Subscriptions;

namespace ClassManager.Api.I.Tests.Endpoints.Subscriptions.When_a_member_of_a_free_brand_reads_their_access;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_plan_and_its_limits_are_returned(ApiFixture fixture)
{
    private const int FreeStudentLimit = 30;

    [Fact]
    public async Task Then_the_plan_and_its_limits_are_returned_Run()
    {
        var business = await fixture.SeedBusinessAsync(PlanCodes.Free);

        var member = await business.HttpClient.GetCurrentMemberAsync();

        member.Subscription!.PlanCode.ShouldBe(PlanCodes.Free);
        member.Subscription.IsActive.ShouldBeTrue();
        member.Subscription.Features.Single(feature => feature.Code == Features.Students).Limit.ShouldBe(FreeStudentLimit);
        member.Subscription.Features.ShouldNotContain(feature => feature.Code == Features.ImportExport);
    }
}
