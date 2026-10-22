using ClassManager.Core.UseCases.Authentication;
using ClassManager.Core.UseCases.Branches;

namespace ClassManager.Api.I.Tests.Infrastructure;

public static class BranchRequests
{
    public const string SwitchBranchRoute = ApiRoutes.Authentication + ApiRoutes.SwitchBranch;

    public static CreateBranchCommand BranchCommand(string name, string? locationName = null, string? locationMapsUrl = null) =>
        new(name, "ES", "Atlantic/Canary", "EUR", locationName, locationMapsUrl);

    public static Task<HttpResponseMessage> PostBranchAsync(this HttpClient httpClient, string name) =>
        httpClient.PostAsJsonAsync(ApiRoutes.OrganizationBranches, BranchCommand(name), ApiRequests.JsonOptions);

    public static async Task<BranchResponse> CreateBranchAsync(this HttpClient httpClient, string name)
    {
        using var response = await httpClient.PostBranchAsync(name);
        response.StatusCode.ShouldBe(HttpStatusCode.Created);
        return (await response.Content.ReadFromJsonAsync<BranchResponse>(ApiRequests.JsonOptions))!;
    }

    public static async Task<List<BranchResponse>> ListBranchesAsync(this HttpClient httpClient) =>
        (await httpClient.GetFromJsonAsync<List<BranchResponse>>(new Uri(ApiRoutes.MyBranches, UriKind.Relative), ApiRequests.JsonOptions))!;

    public static Task<HttpResponseMessage> PostSwitchBranchAsync(this HttpClient httpClient, string refreshToken, Guid businessId, string? kind = null) =>
        httpClient.PostAsJsonAsync(SwitchBranchRoute, new SwitchBranchCommand(refreshToken, businessId, kind), ApiRequests.JsonOptions);

    public static async Task<TokenResponse> SwitchBranchAsync(this HttpClient httpClient, string refreshToken, Guid businessId, string? kind = null)
    {
        using var response = await httpClient.PostSwitchBranchAsync(refreshToken, businessId, kind);
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        return await response.ReadTokensAsync();
    }

    public static Task<HttpResponseMessage> PutBrandOwnerAsync(this HttpClient httpClient, Guid memberId) =>
        httpClient.PutAsync(new Uri($"{ApiRoutes.Members}/{memberId}{ApiRoutes.BrandOwnerSegment}", UriKind.Relative), null);

    public static Task<HttpResponseMessage> DeleteBrandOwnerAsync(this HttpClient httpClient, Guid memberId) =>
        httpClient.DeleteAsync(new Uri($"{ApiRoutes.Members}/{memberId}{ApiRoutes.BrandOwnerSegment}", UriKind.Relative));
}
