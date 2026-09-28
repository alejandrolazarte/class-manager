using ClassManager.Core.UseCases.Members;

namespace ClassManager.Api.I.Tests.Infrastructure;

public static class MemberRequests
{
    public static async Task<CurrentMemberResponse> GetCurrentMemberAsync(this HttpClient httpClient)
    {
        using var response = await httpClient.GetAsync(new Uri(ApiRoutes.Me, UriKind.Relative));
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        return (await response.Content.ReadFromJsonAsync<CurrentMemberResponse>(ApiRequests.JsonOptions))!;
    }
}
