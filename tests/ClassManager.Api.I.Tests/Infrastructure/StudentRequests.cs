using ClassManager.Core.UseCases.Clients;
using ClassManager.Core.UseCases.Students;

namespace ClassManager.Api.I.Tests.Infrastructure;

public static class StudentRequests
{
    public static Task<HttpResponseMessage> PutStudentAsync(this HttpClient httpClient, long studentId, UpdateStudentRequest request) =>
        httpClient.PutAsJsonAsync($"{ApiRoutes.Students}/{studentId}", request, ApiRequests.JsonOptions);

    public static Task<HttpResponseMessage> DeleteStudentAsync(this HttpClient httpClient, long studentId) =>
        httpClient.DeleteAsync(new Uri($"{ApiRoutes.Students}/{studentId}", UriKind.Relative));

    public static async Task<ClientDetailsResponse> GetClientAsync(this HttpClient httpClient, long clientId) =>
        (await httpClient.GetFromJsonAsync<ClientDetailsResponse>(new Uri($"{ApiRoutes.Clients}/{clientId}", UriKind.Relative), ApiRequests.JsonOptions))!;
}
