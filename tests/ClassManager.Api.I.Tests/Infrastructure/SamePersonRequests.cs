using System.Text.Json;
using ClassManager.Core.UseCases.Users;
using Microsoft.EntityFrameworkCore;

namespace ClassManager.Api.I.Tests.Infrastructure;

public static class SamePersonRequests
{
    public static Guid? SamePersonUserIdOf(JsonElement problem) =>
        problem.TryGetProperty(SamePerson.UserIdDetail, out var userId) ? userId.GetGuid() : null;

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
