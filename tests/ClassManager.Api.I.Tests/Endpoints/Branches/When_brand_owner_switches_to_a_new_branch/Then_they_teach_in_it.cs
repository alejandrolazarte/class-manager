using ClassManager.Core.Domain.Businesses;
using ClassManager.Core.Domain.Subscriptions;
using ClassManager.Core.UseCases.Instructors;

namespace ClassManager.Api.I.Tests.Endpoints.Branches.When_brand_owner_switches_to_a_new_branch;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_they_teach_in_it(ApiFixture fixture)
{
    [Fact]
    public async Task Then_they_teach_in_it_Run()
    {
        using var client = fixture.ApiFactory.CreateClient();
        var tokens = await client.SignUpAsync(AuthenticationRequests.SignUpCommand());
        using var ownerClient = fixture.CreateClientWithToken(tokens.AccessToken);
        await fixture.ChangePlanAsync((await ownerClient.GetCurrentMemberAsync()).BusinessId, PlanCodes.Pro);
        var branch = await ownerClient.CreateBranchAsync("DF Valencia");

        var switchedTokens = await client.SwitchBranchAsync(tokens.RefreshToken, branch.BusinessId);

        using var branchClient = fixture.CreateClientWithToken(switchedTokens.AccessToken);
        var instructors = await branchClient.GetFromJsonAsync<List<InstructorResponse>>(
            new Uri(ApiRoutes.Instructors, UriKind.Relative), ApiRequests.JsonOptions);
        var member = await branchClient.GetCurrentMemberAsync();
        var ownerInstructor = instructors!.Single();
        ownerInstructor.FullName.ShouldBe(AuthenticationRequests.OwnerFullName);
        member.BranchRole.ShouldBe(BusinessRole.BranchOwner);
        member.InstructorId.ShouldBe(ownerInstructor.Id);
    }
}
