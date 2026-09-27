using ClassManager.Core.Domain.Sessions;
using ClassManager.Core.UseCases.PrivateLessons;
using ClassManager.Core.UseCases.Sessions;

namespace ClassManager.Api.I.Tests.Infrastructure;

public static class PrivateLessonRequests
{
    public static Task<HttpResponseMessage> PostPrivateLessonAsync(
        this HttpClient httpClient,
        Guid instructorId,
        Guid studentId,
        DateOnly date,
        string startTime = "10:00",
        int repeatWeeks = 1) =>
        httpClient.PostAsJsonAsync(
            ApiRoutes.PrivateLessons,
            new SchedulePrivateLessonCommand(instructorId, [studentId], date, startTime, 45, "Piscina Alboraya", null, repeatWeeks),
            ApiRequests.JsonOptions);

    public static async Task<PrivateLessonResponse> SchedulePrivateLessonAsync(
        this HttpClient httpClient,
        Guid instructorId,
        Guid studentId,
        DateOnly date,
        string startTime = "10:00")
    {
        using var response = await httpClient.PostPrivateLessonAsync(instructorId, studentId, date, startTime);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<List<PrivateLessonResponse>>(ApiRequests.JsonOptions))![0];
    }

    public static Task<HttpResponseMessage> PutPrivateLessonAttendanceAsync(
        this HttpClient httpClient, Guid privateLessonId, Guid studentId, AttendanceStatus? status) =>
        httpClient.PutAsJsonAsync(
            $"{ApiRoutes.PrivateLessons}/{privateLessonId}{ApiRoutes.Attendance}/{studentId}",
            new RecordAttendanceRequest(status),
            ApiRequests.JsonOptions);
}
