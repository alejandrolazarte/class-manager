using System.Globalization;

namespace ClassManager.Api.I.Tests.Infrastructure;

public static class AbsenceRequests
{
    private const string IsoDateFormat = "yyyy-MM-dd";

    public static string AbsencePath(Guid studentId, Guid classGroupId, DateOnly sessionDate) =>
        $"{ApiRoutes.StudentApp}{ApiRoutes.AccountStudents}/{studentId}{ApiRoutes.Absences}/{classGroupId}/{sessionDate.ToString(IsoDateFormat, CultureInfo.InvariantCulture)}";

    public static Task<HttpResponseMessage> PutAbsenceAsync(this HttpClient httpClient, Guid studentId, Guid classGroupId, DateOnly sessionDate) =>
        httpClient.PutAsJsonAsync(AbsencePath(studentId, classGroupId, sessionDate), new { }, ApiRequests.JsonOptions);

    public static Task<HttpResponseMessage> DeleteAbsenceAsync(this HttpClient httpClient, Guid studentId, Guid classGroupId, DateOnly sessionDate) =>
        httpClient.DeleteAsync(new Uri(AbsencePath(studentId, classGroupId, sessionDate), UriKind.Relative));
}
