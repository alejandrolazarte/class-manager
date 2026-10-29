using ClassManager.Core.UseCases.ClassGroups;
using ClassManager.Core.UseCases.StudentApp;

namespace ClassManager.Api.I.Tests.Infrastructure;

public static class StudentAppClassPackRequests
{
    public const string OtherClassName = "Otra clase";

    public static Task<StudentAppClassPacksResponse?> GetStudentAppClassPacksAsync(this HttpClient httpClient) =>
        httpClient.GetFromJsonAsync<StudentAppClassPacksResponse>(
            new Uri($"{ApiRoutes.StudentApp}{ApiRoutes.StudentAppClassPacks}", UriKind.Relative), ApiRequests.JsonOptions);

    public static Task<ClassGroupResponse> CreateOtherClassAsync(this StudentAppScenario scenario) =>
        scenario.Instructors.Business.HttpClient.CreateClassGroupAsync(
            ClassGroupRequests.ClassGroupFor(scenario.Instructors.InstructorId, [InstructorScenario.ClassDate.DayOfWeek], "10:00") with { Name = OtherClassName });
}
