using System.Globalization;
using ClassManager.Core.UseCases.ClassPacks;
using ClassManager.Core.UseCases.StudentApp;

namespace ClassManager.Api.I.Tests.Infrastructure;

public static class StudentAppPlanRequests
{
    public const string HiddenPlanName = "Plan del staff";

    public static Task<HttpResponseMessage> PutStudentAppPlanAsync(this HttpClient httpClient, long studentId, long? classPackId) =>
        httpClient.PutAsJsonAsync(
            ApiRoutes.StudentApp + ApiRoutes.StudentAppStudentPlan.Replace("{studentId:long}", studentId.ToString(CultureInfo.InvariantCulture)),
            new ChooseStudentAppPlanRequest(classPackId),
            ApiRequests.JsonOptions);

    public static async Task ChooseStudentAppPlanAsync(this HttpClient httpClient, long studentId, long? classPackId)
    {
        using var response = await httpClient.PutStudentAppPlanAsync(studentId, classPackId);
        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
    }

    public static async Task<ClassPackResponse> CreateHiddenMonthlyPlanAsync(this HttpClient httpClient)
    {
        using var response = await httpClient.PostAsJsonAsync(
            ApiRoutes.ClassPacks,
            new CreateClassPackCommand(HiddenPlanName, null, 9000m, null, CoversAnyGroupClass: true, RenewsMonthly: true, IsVisibleInApp: false),
            ApiRequests.JsonOptions);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<ClassPackResponse>(ApiRequests.JsonOptions))!;
    }

    public static async Task<StudentAppPlanStudentResponse> PlanStudentAsync(this HttpClient httpClient, long studentId) =>
        (await httpClient.GetStudentAppShopAsync()).PlanStudents.Single(student => student.StudentId == studentId);
}
