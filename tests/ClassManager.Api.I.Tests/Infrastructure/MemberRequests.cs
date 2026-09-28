using ClassManager.Core.Domain.Businesses;
using ClassManager.Core.UseCases.Authentication;
using ClassManager.Core.UseCases.Members;

namespace ClassManager.Api.I.Tests.Infrastructure;

public static class MemberRequests
{
    public const string InviteeFullName = "Marcos Díaz";
    public const string InviteePassword = "another long passphrase";
    public const string WebAppAcceptInvitationUrl = "http://localhost:8081/accept-invitation?token=";

    public const string InvitationsRoute = ApiRoutes.Members + ApiRoutes.InvitationsSegment;
    public const string AcceptInvitationRoute = ApiRoutes.Authentication + ApiRoutes.AcceptInvitation;

    public static string UniqueInviteeEmail() => $"coach-{Guid.NewGuid():N}@example.com";

    public static async Task<CurrentMemberResponse> GetCurrentMemberAsync(this HttpClient httpClient)
    {
        using var response = await httpClient.GetAsync(new Uri(ApiRoutes.Me, UriKind.Relative));
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        return (await response.Content.ReadFromJsonAsync<CurrentMemberResponse>(ApiRequests.JsonOptions))!;
    }

    public static Task<HttpResponseMessage> PostInvitationAsync(this HttpClient httpClient, string email, BusinessRole role, Guid? instructorId = null) =>
        httpClient.PostAsJsonAsync(InvitationsRoute, new InviteMemberCommand(email, role, instructorId), ApiRequests.JsonOptions);

    public static async Task<InvitationResponse> InviteAsync(this HttpClient httpClient, string email, BusinessRole role, Guid? instructorId = null)
    {
        using var response = await httpClient.PostInvitationAsync(email, role, instructorId);
        response.StatusCode.ShouldBe(HttpStatusCode.Created);
        return (await response.Content.ReadFromJsonAsync<InvitationResponse>(ApiRequests.JsonOptions))!;
    }

    public static string InvitationTokenSentTo(this RecordingEmailSender emailSender, string email)
    {
        var body = emailSender.SentTo(email)[^1].TextBody;
        var link = body.Split('\n').Single(line => line.StartsWith(WebAppAcceptInvitationUrl, StringComparison.Ordinal));
        return Uri.UnescapeDataString(link[WebAppAcceptInvitationUrl.Length..]);
    }

    public static Task<HttpResponseMessage> PostAcceptInvitationAsync(this HttpClient httpClient, string token) =>
        httpClient.PostAsJsonAsync(
            AcceptInvitationRoute,
            new AcceptInvitationCommand(token, InviteeFullName, InviteePassword),
            ApiRequests.JsonOptions);

    public static async Task<TokenResponse> AcceptInvitationAsync(this HttpClient httpClient, string token)
    {
        using var response = await httpClient.PostAcceptInvitationAsync(token);
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        return await response.ReadTokensAsync();
    }

    public static async Task<TeamResponse> GetTeamAsync(this HttpClient httpClient)
    {
        using var response = await httpClient.GetAsync(new Uri(ApiRoutes.Members, UriKind.Relative));
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        return (await response.Content.ReadFromJsonAsync<TeamResponse>(ApiRequests.JsonOptions))!;
    }

    public static Task<HttpResponseMessage> PutMemberAsync(this HttpClient httpClient, Guid memberId, BusinessRole role, Guid? instructorId = null) =>
        httpClient.PutAsJsonAsync($"{ApiRoutes.Members}/{memberId}", new ChangeMemberRoleRequest(role, instructorId), ApiRequests.JsonOptions);

    public static Task<HttpResponseMessage> DeleteMemberAsync(this HttpClient httpClient, Guid memberId) =>
        httpClient.DeleteAsync(new Uri($"{ApiRoutes.Members}/{memberId}", UriKind.Relative));
}
