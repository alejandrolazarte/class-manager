using System.Net.Http.Headers;
using ClassManager.Core.UseCases.Authentication;
using ClassManager.Core.UseCases.UserPhotos;
using SkiaSharp;

namespace ClassManager.Api.I.Tests.Infrastructure;

public sealed record GuardedChildScenario(SeededBusiness Business, HttpClient Guardian, HttpClient Child, long ChildStudentId) : IDisposable
{
    public void Dispose()
    {
        Guardian.Dispose();
        Child.Dispose();
    }
}

public static class UserPhotoRequests
{
    public const int LandscapeWidth = 600;
    public const int LandscapeHeight = 400;

    private const string PhotoFileName = "me.png";
    private const string PngContentType = "image/png";
    private const int PngQuality = 100;

    public static byte[] LandscapePng()
    {
        using var bitmap = new SKBitmap(LandscapeWidth, LandscapeHeight);
        using (var canvas = new SKCanvas(bitmap))
        {
            canvas.Clear(SKColors.SteelBlue);
        }

        using var image = SKImage.FromBitmap(bitmap);
        using var encoded = image.Encode(SKEncodedImageFormat.Png, PngQuality);
        return encoded.ToArray();
    }

    public static Task<HttpResponseMessage> PutMyPhotoAsync(this HttpClient httpClient, byte[] content)
    {
        var fileContent = new ByteArrayContent(content);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue(PngContentType);
        return httpClient.PutAsync(
            new Uri(ApiRoutes.MyPhoto, UriKind.Relative),
            new MultipartFormDataContent { { fileContent, ImportRequests.FileFieldName, PhotoFileName } });
    }

    public static async Task<MyPhotoResponse> ChangeMyPhotoAsync(this HttpClient httpClient)
    {
        using var response = await httpClient.PutMyPhotoAsync(LandscapePng());
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        return (await response.Content.ReadFromJsonAsync<MyPhotoResponse>(ApiRequests.JsonOptions))!;
    }

    public static Task<MyPhotoResponse?> GetMyPhotoAsync(this HttpClient httpClient) =>
        httpClient.GetFromJsonAsync<MyPhotoResponse>(new Uri(ApiRoutes.MyPhoto, UriKind.Relative), ApiRequests.JsonOptions);

    public static Task<HttpResponseMessage> DeleteMyPhotoAsync(this HttpClient httpClient) =>
        httpClient.DeleteAsync(new Uri(ApiRoutes.MyPhoto, UriKind.Relative));

    public static async Task<IReadOnlyList<PhotoApprovalResponse>> ListPhotoApprovalsAsync(this HttpClient httpClient) =>
        (await httpClient.GetFromJsonAsync<List<PhotoApprovalResponse>>(
            new Uri(ApiRoutes.StudentApp + ApiRoutes.StudentAppPhotoApprovals, UriKind.Relative), ApiRequests.JsonOptions))!;

    public static Task<HttpResponseMessage> PostPhotoApprovalAsync(this HttpClient httpClient, long photoId) =>
        httpClient.PostAsync(new Uri($"{ApiRoutes.StudentApp}{ApiRoutes.StudentAppPhotoApprovals}/{photoId}{ApiRoutes.Authorization}", UriKind.Relative), null);

    public static Task<HttpResponseMessage> PostPhotoRefusalAsync(this HttpClient httpClient, long photoId) =>
        httpClient.PostAsync(new Uri($"{ApiRoutes.StudentApp}{ApiRoutes.StudentAppPhotoApprovals}/{photoId}{ApiRoutes.Refusal}", UriKind.Relative), null);

    public static async Task<GuardedChildScenario> SeedGuardedChildAsync(this ApiFixture fixture)
    {
        var business = await fixture.SeedBusinessAsync();
        var clientEmail = StudentAppRequests.UniqueStudentEmail();
        var family = await business.HttpClient.RegisterFamilyAsync(clientEmail);
        var guardian = await fixture.SignInClientOfFamilyAsync(business.HttpClient, family.Id, clientEmail);
        var childEmail = StudentAppRequests.UniqueStudentEmail();
        (await business.HttpClient.PostChildAppInvitationAsync(
            family.Id, family.Students.Single().Id, childEmail, StudentAppRequests.ElevenYearsOld)).EnsureSuccessStatusCode();
        using var anonymous = fixture.ApiFactory.CreateClient();
        (await anonymous.PostGiveGuardianConsentAsync(fixture.ApiFactory.EmailTransport.GuardianConsentTokenSentTo(clientEmail))).EnsureSuccessStatusCode();
        using var accepted = await anonymous.PostAcceptStudentAppInvitationAsync(fixture.ApiFactory.EmailTransport.StudentAppInvitationTokenSentTo(childEmail));
        accepted.EnsureSuccessStatusCode();
        var tokens = (await accepted.Content.ReadFromJsonAsync<TokenResponse>(ApiRequests.JsonOptions))!;
        return new GuardedChildScenario(business, guardian, fixture.CreateClientWithToken(tokens.AccessToken), family.Students.Single().Id);
    }

    public static async Task<SKImage> DownloadImageAsync(this ApiFixture fixture, string url)
    {
        var content = await fixture.AnonymousClient.GetByteArrayAsync(new Uri(url));
        return SKImage.FromEncodedData(content);
    }
}
