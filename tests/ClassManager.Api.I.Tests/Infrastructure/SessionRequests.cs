using System.Globalization;
using ClassManager.Core.Domain.Sessions;
using ClassManager.Core.UseCases.Sessions;

namespace ClassManager.Api.I.Tests.Infrastructure;

public static class SessionRequests
{
    private const string IsoDateFormat = "yyyy-MM-dd";

    public static string SessionPath(Guid classGroupId, DateOnly sessionDate) =>
        $"{ApiRoutes.ClassGroups}/{classGroupId}{ApiRoutes.SessionsSegment}/{sessionDate.ToString(IsoDateFormat, CultureInfo.InvariantCulture)}";

    public static Task<HttpResponseMessage> PutAttendanceAsync(
        this HttpClient httpClient, Guid classGroupId, DateOnly sessionDate, Guid studentId, AttendanceStatus? status) =>
        httpClient.PutAsJsonAsync(
            $"{SessionPath(classGroupId, sessionDate)}{ApiRoutes.Attendance}/{studentId}",
            new RecordAttendanceRequest(status),
            ApiRequests.JsonOptions);

    public static Task<SessionDetailsResponse?> GetSessionAsync(this HttpClient httpClient, Guid classGroupId, DateOnly sessionDate) =>
        httpClient.GetFromJsonAsync<SessionDetailsResponse>(new Uri(SessionPath(classGroupId, sessionDate), UriKind.Relative), ApiRequests.JsonOptions);

    public static Task<List<DaySessionResponse>?> GetDayAsync(this HttpClient httpClient, DateOnly sessionDate) =>
        httpClient.GetFromJsonAsync<List<DaySessionResponse>>(
            new Uri($"{ApiRoutes.Sessions}?date={sessionDate.ToString(IsoDateFormat, CultureInfo.InvariantCulture)}", UriKind.Relative),
            ApiRequests.JsonOptions);

    public static Task<MonthCalendarResponse?> GetMonthCalendarAsync(this HttpClient httpClient, string month) =>
        httpClient.GetFromJsonAsync<MonthCalendarResponse>(
            new Uri($"{ApiRoutes.Sessions}{ApiRoutes.Calendar}?month={month}", UriKind.Relative),
            ApiRequests.JsonOptions);
}
