using System.Globalization;
using ClassManager.Core.UseCases.ClassGroups;
using ClassManager.Core.UseCases.StudentApp;

namespace ClassManager.Api.I.Tests.Infrastructure;

public static class ClassHistoryRequests
{
    private const string MonthFormat = "yyyy-MM";

    public static readonly string CurrentMonth = InstructorScenario.ClassDate.ToString(MonthFormat, CultureInfo.InvariantCulture);

    public static string ClassHistoryPath(long studentId, string month) =>
        $"{ApiRoutes.StudentApp}{ApiRoutes.AccountStudents}/{studentId}{ApiRoutes.ClassHistory}?month={month}";

    public static Task<HttpResponseMessage> GetClassHistoryResponseAsync(this HttpClient httpClient, long studentId, string month) =>
        httpClient.GetAsync(new Uri(ClassHistoryPath(studentId, month), UriKind.Relative));

    public static Task<StudentAppClassHistoryResponse?> GetClassHistoryAsync(this HttpClient httpClient, long studentId, string month) =>
        httpClient.GetFromJsonAsync<StudentAppClassHistoryResponse>(new Uri(ClassHistoryPath(studentId, month), UriKind.Relative), ApiRequests.JsonOptions);

    public static async Task<ClassGroupResponse> EnrollInWeeklyClassSinceAsync(this StudentAppScenario scenario, DateOnly startDate)
    {
        var owner = scenario.Instructors.Business.HttpClient;
        var weekly = await owner.CreateClassGroupAsync(
            ClassGroupRequests.ClassGroupFor(scenario.Instructors.InstructorId, [InstructorScenario.ClassDate.DayOfWeek], "08:00") with { Name = "Clase semanal" });
        await owner.EnrollAsync(weekly.Id, scenario.Instructors.InstructorStudentId, startDate);
        return weekly;
    }
}
