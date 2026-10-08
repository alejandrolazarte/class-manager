using ClassManager.Core.UseCases.ClassGroups;
using ClassManager.Core.UseCases.Instructors;

namespace ClassManager.Api.I.Tests.Infrastructure;

public static class ClassGroupRequests
{
    public const string InstructorFullName = "Laura Gómez";
    public const string ClassGroupName = "Natación inicial";

    public static Task<HttpResponseMessage> PostInstructorAsync(this HttpClient httpClient, string fullName = InstructorFullName) =>
        httpClient.PostAsJsonAsync(ApiRoutes.Instructors, new CreateInstructorCommand(fullName), ApiRequests.JsonOptions);

    public static async Task<InstructorResponse> CreateInstructorAsync(this HttpClient httpClient, string fullName = InstructorFullName)
    {
        using var response = await httpClient.PostInstructorAsync(fullName);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<InstructorResponse>(ApiRequests.JsonOptions))!;
    }

    public static ClassGroupDetails ClassGroupFor(
        Guid instructorId,
        IReadOnlyList<DayOfWeek>? weekdays = null,
        string startTime = "18:00",
        int durationMinutes = 45) =>
        new(ClassGroupName, instructorId, weekdays ?? [DayOfWeek.Tuesday, DayOfWeek.Thursday], startTime, durationMinutes, 8, "Pileta chica");

    public static Task<HttpResponseMessage> PostClassGroupAsync(this HttpClient httpClient, ClassGroupDetails details) =>
        httpClient.PostAsJsonAsync(ApiRoutes.ClassGroups, details, ApiRequests.JsonOptions);

    public static async Task<ClassGroupResponse> CreateClassGroupAsync(this HttpClient httpClient, ClassGroupDetails details)
    {
        using var response = await httpClient.PostClassGroupAsync(details);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<ClassGroupResponse>(ApiRequests.JsonOptions))!;
    }

    public static async Task ShareClassGroupMaterialAsync(this HttpClient httpClient, ClassGroupResponse classGroup, string materialUrl)
    {
        var details = new ClassGroupDetails(
            classGroup.Name,
            classGroup.InstructorId,
            classGroup.Weekdays,
            classGroup.StartTime,
            classGroup.DurationMinutes,
            classGroup.Capacity,
            classGroup.Location,
            materialUrl);
        using var response = await httpClient.PutAsJsonAsync($"{ApiRoutes.ClassGroups}/{classGroup.Id}", details, ApiRequests.JsonOptions);
        response.EnsureSuccessStatusCode();
    }
}
