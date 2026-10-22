using ClassManager.Core.UseCases.ClassGroups;
using ClassManager.Core.UseCases.Enrollments;
using ClassManager.Core.UseCases.Students;

namespace ClassManager.Api.I.Tests.Infrastructure;

public static class EnrollmentRequests
{
    public static readonly DateOnly Today = DateOnly.FromDateTime(BusinessApiFactory.Now.UtcDateTime);

    public static async Task<long> RegisterStudentAsync(this HttpClient httpClient, string studentFullName = ApiRequests.StudentFullName, string phoneNumber = ApiRequests.ClientPhoneNumber)
    {
        var client = await httpClient.RegisterClientAsync(phoneNumber: phoneNumber, students: [new NewStudent(studentFullName, null, null)]);
        return client.Students[0].Id;
    }

    public static async Task<ClassGroupResponse> CreateClassGroupWithInstructorAsync(this HttpClient httpClient, int capacity = 8)
    {
        var instructor = await httpClient.CreateInstructorAsync();
        return await httpClient.CreateClassGroupAsync(ClassGroupRequests.ClassGroupFor(instructor.Id) with { Capacity = capacity });
    }

    public static Task<HttpResponseMessage> PostEnrollmentAsync(
        this HttpClient httpClient, long classGroupId, long studentId, DateOnly? startDate = null, bool allowOutsidePack = false) =>
        httpClient.PostAsJsonAsync(
            $"{ApiRoutes.ClassGroups}/{classGroupId}{ApiRoutes.EnrollmentsSegment}",
            new EnrollStudentRequest(studentId, startDate, allowOutsidePack),
            ApiRequests.JsonOptions);

    public static async Task<EnrollmentResponse> EnrollAsync(this HttpClient httpClient, long classGroupId, long studentId, DateOnly? startDate = null)
    {
        using var response = await httpClient.PostEnrollmentAsync(classGroupId, studentId, startDate);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<EnrollmentResponse>(ApiRequests.JsonOptions))!;
    }

    public static Task<List<RosterEntryResponse>?> GetRosterAsync(this HttpClient httpClient, long classGroupId) =>
        httpClient.GetFromJsonAsync<List<RosterEntryResponse>>(
            new Uri($"{ApiRoutes.ClassGroups}/{classGroupId}{ApiRoutes.EnrollmentsSegment}", UriKind.Relative), ApiRequests.JsonOptions);
}
