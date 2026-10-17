using ClassManager.Core.UseCases.Accounts;

namespace ClassManager.Api.I.Tests.Infrastructure;

public static class AccountDeletionRequests
{
    public static Task<HttpResponseMessage> PostAccountDeletionAsync(this HttpClient httpClient, string currentPassword) =>
        httpClient.PostAsJsonAsync(ApiRoutes.MyAccountDeletion, new DeleteMyAccountCommand(currentPassword), ApiRequests.JsonOptions);

    public static async Task DeleteAccountAsync(this StudentAppScenario scenario)
    {
        using var response = await scenario.Student.PostAccountDeletionAsync(StudentAppRequests.StudentAppPassword);
        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
    }
}
