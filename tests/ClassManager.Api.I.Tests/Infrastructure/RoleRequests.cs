using ClassManager.Core.Domain.Businesses;
using ClassManager.Core.Domain.Roles;
using ClassManager.Core.UseCases.Members;
using ClassManager.Core.UseCases.Roles;

namespace ClassManager.Api.I.Tests.Infrastructure;

public static class RoleRequests
{
    public const string CustomRoleName = "Instructor que cobra";

    public static Task<HttpResponseMessage> PostRoleAsync(
        this HttpClient httpClient, string name, IReadOnlyList<string> permissions, string? copiedFrom = null) =>
        httpClient.PostAsJsonAsync(ApiRoutes.Roles, new CreateRoleCommand(name, permissions, copiedFrom), ApiRequests.JsonOptions);

    public static async Task<RoleResponse> CreateRoleAsync(
        this HttpClient httpClient, string name, IReadOnlyList<string> permissions, string? copiedFrom = null)
    {
        using var response = await httpClient.PostRoleAsync(name, permissions, copiedFrom);
        response.StatusCode.ShouldBe(HttpStatusCode.Created);
        return (await response.Content.ReadFromJsonAsync<RoleResponse>(ApiRequests.JsonOptions))!;
    }

    public static async Task<List<RoleResponse>> ListRolesAsync(this HttpClient httpClient) =>
        (await httpClient.GetFromJsonAsync<List<RoleResponse>>(new Uri(ApiRoutes.Roles, UriKind.Relative), ApiRequests.JsonOptions))!;

    public static Task<HttpResponseMessage> DeleteRoleAsync(this HttpClient httpClient, Guid roleId) =>
        httpClient.DeleteAsync(new Uri($"{ApiRoutes.Roles}/{roleId}", UriKind.Relative));

    public static Task<HttpResponseMessage> PostCustomRoleInvitationAsync(
        this HttpClient httpClient, string email, Guid customRoleId, Guid? instructorId = null) =>
        httpClient.PostAsJsonAsync(
            MemberRequests.InvitationsRoute,
            new InviteMemberCommand(email, BusinessRole.Custom, instructorId, customRoleId),
            ApiRequests.JsonOptions);

    public static async Task<CustomRole> SeedCustomRoleAsync(this ApiFixture fixture, Guid businessId, IReadOnlyList<string> permissions)
    {
        var role = CustomRole.Create(CustomRoleName, permissions, null, BusinessApiFactory.Now).Value!;
        await using var context = fixture.CreateDbContext(businessId);
        context.CustomRoles.Add(role);
        await context.SaveChangesAsync();
        return role;
    }
}
