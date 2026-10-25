using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Domain.Businesses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ClassManager.Api.I.Tests.Infrastructure;

public sealed record UnlinkedOwnerScenario(HttpClient OwnerClient, long BusinessId, long OwnerInstructorId) : IDisposable
{
    public void Dispose() => OwnerClient.Dispose();
}

public static class UnlinkedOwnerScenarios
{
    private sealed record InstructorIdentity(long Id);

    public static async Task<UnlinkedOwnerScenario> SeedOwnerUnlinkedFromTheirInstructorAsync(this ApiFixture fixture)
    {
        using var anonymous = fixture.ApiFactory.CreateClient();
        var tokens = await anonymous.SignUpAsync(fixture.ApiFactory.EmailTransport, AuthenticationRequests.SignUpCommand());
        var ownerClient = fixture.CreateClientWithToken(tokens.AccessToken);
        var businessId = (await ownerClient.ListBranchesAsync()).Single().BusinessId;
        var instructors = await ownerClient.GetFromJsonAsync<List<InstructorIdentity>>(
            new Uri(ApiRoutes.Instructors, UriKind.Relative), ApiRequests.JsonOptions);
        await using var context = fixture.CreateDbContext(businessId);
        await context.BusinessMembers.ExecuteUpdateAsync(setters => setters.SetProperty(member => member.InstructorId, (long?)null));

        return new UnlinkedOwnerScenario(ownerClient, businessId, instructors!.Single().Id);
    }

    public static async Task<string> SeedInvitationForOwnerInstructorAsync(this ApiFixture fixture, UnlinkedOwnerScenario scenario, string email)
    {
        var token = fixture.ApiFactory.Services.GetRequiredService<ISecretTokenGenerator>().Create();
        var invitation = MemberInvitation.Create(
            email, MemberRole.System(BusinessRole.Instructor), scenario.OwnerInstructorId, token.Hash, null, BusinessApiFactory.Now).Value!;
        await using var context = fixture.CreateDbContext(scenario.BusinessId);
        context.Add(invitation);
        await context.SaveChangesAsync();
        return token.Value;
    }
}
