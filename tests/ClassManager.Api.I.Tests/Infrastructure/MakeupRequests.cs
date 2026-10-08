using System.Globalization;
using ClassManager.Core.UseCases.StudentApp;

namespace ClassManager.Api.I.Tests.Infrastructure;

public static class MakeupRequests
{
    private const string IsoDateFormat = "yyyy-MM-dd";

    public static string MakeupsPath(Guid studentId) =>
        $"{ApiRoutes.StudentApp}{ApiRoutes.AccountStudents}/{studentId}{ApiRoutes.Makeups}";

    public static string MakeupPath(Guid studentId, Guid classGroupId, DateOnly sessionDate) =>
        $"{MakeupsPath(studentId)}/{classGroupId}/{sessionDate.ToString(IsoDateFormat, CultureInfo.InvariantCulture)}";

    public static Task<StudentAppMakeupsResponse?> GetMakeupsAsync(this HttpClient httpClient, Guid studentId) =>
        httpClient.GetFromJsonAsync<StudentAppMakeupsResponse>(new Uri(MakeupsPath(studentId), UriKind.Relative), ApiRequests.JsonOptions);

    public static Task<HttpResponseMessage> PutMakeupAsync(this HttpClient httpClient, Guid studentId, Guid classGroupId, DateOnly sessionDate) =>
        httpClient.PutAsJsonAsync(MakeupPath(studentId, classGroupId, sessionDate), new { }, ApiRequests.JsonOptions);

    public static Task<HttpResponseMessage> DeleteMakeupAsync(this HttpClient httpClient, Guid studentId, Guid classGroupId, DateOnly sessionDate) =>
        httpClient.DeleteAsync(new Uri(MakeupPath(studentId, classGroupId, sessionDate), UriKind.Relative));

    public static async Task NoticeAndBookOtherClassAsync(this StudentAppScenario scenario)
    {
        var instructors = scenario.Instructors;
        (await scenario.Student.PutAbsenceAsync(instructors.InstructorStudentId, instructors.InstructorClassGroup.Id, InstructorScenario.ClassDate)).EnsureSuccessStatusCode();
        (await scenario.Student.PutMakeupAsync(instructors.InstructorStudentId, instructors.OtherClassGroup.Id, InstructorScenario.ClassDate)).EnsureSuccessStatusCode();
    }
}
