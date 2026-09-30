using System.Globalization;
using ClassManager.Core.UseCases.Families;

namespace ClassManager.Api.I.Tests.Infrastructure;

public static class MakeupRequests
{
    private const string IsoDateFormat = "yyyy-MM-dd";

    public static string MakeupsPath(Guid studentId) =>
        $"{ApiRoutes.Family}{ApiRoutes.FamilyStudents}/{studentId}{ApiRoutes.Makeups}";

    public static string MakeupPath(Guid studentId, Guid classGroupId, DateOnly sessionDate) =>
        $"{MakeupsPath(studentId)}/{classGroupId}/{sessionDate.ToString(IsoDateFormat, CultureInfo.InvariantCulture)}";

    public static Task<FamilyMakeupsResponse?> GetMakeupsAsync(this HttpClient httpClient, Guid studentId) =>
        httpClient.GetFromJsonAsync<FamilyMakeupsResponse>(new Uri(MakeupsPath(studentId), UriKind.Relative), ApiRequests.JsonOptions);

    public static Task<HttpResponseMessage> PutMakeupAsync(this HttpClient httpClient, Guid studentId, Guid classGroupId, DateOnly sessionDate) =>
        httpClient.PutAsJsonAsync(MakeupPath(studentId, classGroupId, sessionDate), new { }, ApiRequests.JsonOptions);

    public static Task<HttpResponseMessage> DeleteMakeupAsync(this HttpClient httpClient, Guid studentId, Guid classGroupId, DateOnly sessionDate) =>
        httpClient.DeleteAsync(new Uri(MakeupPath(studentId, classGroupId, sessionDate), UriKind.Relative));

    public static async Task NoticeAndBookOtherClassAsync(this FamilyScenario scenario)
    {
        var coaches = scenario.Coaches;
        (await scenario.Family.PutAbsenceAsync(coaches.CoachStudentId, coaches.CoachClassGroup.Id, CoachScenario.ClassDate)).EnsureSuccessStatusCode();
        (await scenario.Family.PutMakeupAsync(coaches.CoachStudentId, coaches.OtherClassGroup.Id, CoachScenario.ClassDate)).EnsureSuccessStatusCode();
    }
}
