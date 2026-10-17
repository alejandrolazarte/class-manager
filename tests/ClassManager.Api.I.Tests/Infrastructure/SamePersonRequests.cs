using System.Text.Json;
using ClassManager.Core.UseCases.Clients;
using ClassManager.Core.UseCases.Instructors;
using ClassManager.Core.UseCases.Students;
using ClassManager.Core.UseCases.Users;
using Microsoft.EntityFrameworkCore;

namespace ClassManager.Api.I.Tests.Infrastructure;

public static class SamePersonRequests
{
    public static Guid? SamePersonUserIdOf(JsonElement problem) =>
        problem.TryGetProperty(SamePerson.UserIdDetail, out var userId) ? userId.GetGuid() : null;

    public static Task<HttpResponseMessage> PostInstructorAsync(this HttpClient httpClient, string fullName, string email, Guid? samePersonUserId = null) =>
        httpClient.PostAsJsonAsync(ApiRoutes.Instructors, new CreateInstructorCommand(fullName, email, samePersonUserId), ApiRequests.JsonOptions);

    public static async Task<InstructorResponse> CreateInstructorAsync(this HttpClient httpClient, string fullName, string email)
    {
        using var response = await httpClient.PostInstructorAsync(fullName, email);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<InstructorResponse>(ApiRequests.JsonOptions))!;
    }

    public static Task<HttpResponseMessage> PostStudentAsync(this HttpClient httpClient, Guid clientId, string fullName, string email, Guid? samePersonUserId = null) =>
        httpClient.PostAsJsonAsync(
            $"{ApiRoutes.Clients}/{clientId}{ApiRoutes.StudentsOfClient}",
            new NewStudent(fullName, null, null, email, samePersonUserId),
            ApiRequests.JsonOptions);

    public static Task<HttpResponseMessage> PostClientWithEmailAsync(this HttpClient httpClient, string fullName, string email, Guid? samePersonUserId = null) =>
        httpClient.PostAsJsonAsync(
            ApiRoutes.Clients,
            new RegisterClientCommand(fullName, "11 6677-8899", email, null, [new NewStudent("Tomás Gómez", null, null)], samePersonUserId),
            ApiRequests.JsonOptions);

    public static Task<HttpResponseMessage> PostSeparationAsync(this HttpClient httpClient, string route, string fullName, string? email = null) =>
        httpClient.PostAsJsonAsync($"{route}{ApiRoutes.Separation}", new SeparatePersonRequest(fullName, email), ApiRequests.JsonOptions);

    public static async Task<(Guid InstructorId, Guid StudentUserId)> SeedInstructorWhoIsAStudentAsync(this ApiFixture fixture, StudentAppScenario scenario)
    {
        var studentUserId = UserRows.UserIdOf(scenario.Tokens.AccessToken)!.Value;
        using var response = await scenario.Instructors.Business.HttpClient.PostInstructorAsync("Ana Gómez", scenario.Email, studentUserId);
        response.StatusCode.ShouldBe(HttpStatusCode.Created);
        var instructor = (await response.Content.ReadFromJsonAsync<InstructorResponse>(ApiRequests.JsonOptions))!;
        return (instructor.Id, studentUserId);
    }

    public static async Task<Guid> ClientIdOfStudentAsync(this ApiFixture fixture, Guid businessId, Guid studentId)
    {
        await using var context = fixture.CreateDbContext(businessId);
        return await context.Students.Where(student => student.Id == studentId).Select(student => student.ClientId).SingleAsync();
    }

    public static async Task<Guid> UserIdOfInstructorAsync(this ApiFixture fixture, Guid businessId, Guid instructorId)
    {
        await using var context = fixture.CreateDbContext(businessId);
        return await context.Instructors.Where(instructor => instructor.Id == instructorId).Select(instructor => instructor.UserId).SingleAsync();
    }
}
