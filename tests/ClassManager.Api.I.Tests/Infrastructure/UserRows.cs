using System.Globalization;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Domain.Clients;
using ClassManager.Infrastructure.Persistence;
using ClassManager.Security.Tokens;
using Microsoft.EntityFrameworkCore;

namespace ClassManager.Api.I.Tests.Infrastructure;

public static class UserRows
{
    public static Guid AccountIdOf(string accessToken) =>
        Guid.Parse(AuthenticationRequests.AccessTokenClaim(accessToken, SecurityClaimTypes.Subject)!);

    public static long? UserIdOf(string accessToken) =>
        AuthenticationRequests.AccessTokenClaim(accessToken, SessionClaimTypes.UserId) is { } userId ? long.Parse(userId, CultureInfo.InvariantCulture) : null;

    public static async Task<long?> FindUserIdByAccountAsync(this ApiFixture fixture, long businessId, Guid accountId)
    {
        await using var context = fixture.CreateDbContext(businessId);
        return await context.Users
            .Where(user => user.AccountId == accountId)
            .Select(user => (long?)user.Id)
            .SingleOrDefaultAsync();
    }

    public static async Task ChangeUserOfClientAsync(
        this ApiFixture fixture,
        long businessId,
        long clientId,
        string fullName,
        string? email = null,
        string? normalizedPhoneNumber = null)
    {
        await using var context = fixture.CreateDbContext(businessId);
        var userId = await context.Clients.Where(client => client.Id == clientId).Select(client => client.UserId).SingleAsync();
        await ChangeAsync(context, userId, fullName, email, normalizedPhoneNumber);
    }

    public static async Task ChangeUserOfStudentAsync(this ApiFixture fixture, long businessId, long studentId, string fullName, string? email = null)
    {
        await using var context = fixture.CreateDbContext(businessId);
        var userId = await context.Students.Where(student => student.Id == studentId).Select(student => student.UserId).SingleAsync();
        await ChangeAsync(context, userId, fullName, email, null);
    }

    public static async Task ChangeUserOfInstructorAsync(this ApiFixture fixture, long businessId, long instructorId, string fullName)
    {
        await using var context = fixture.CreateDbContext(businessId);
        var userId = await context.Instructors.Where(instructor => instructor.Id == instructorId).Select(instructor => instructor.UserId).SingleAsync();
        await ChangeAsync(context, userId, fullName, null, null);
    }

    public static async Task RenameUserAsync(this ApiFixture fixture, long businessId, long userId, string fullName)
    {
        await using var context = fixture.CreateDbContext(businessId);
        await ChangeAsync(context, userId, fullName, null, null);
    }

    private static async Task ChangeAsync(AppDbContext context, long? userId, string fullName, string? email, string? normalizedPhoneNumber)
    {
        var user = await context.Users.SingleAsync(user => user.Id == userId);
        user.Rename(fullName);
        if (email is not null)
        {
            user.ChangeEmail(email);
        }

        if (normalizedPhoneNumber is not null)
        {
            user.ChangePhoneNumber(PhoneNumber.FromNormalized(normalizedPhoneNumber));
        }

        await context.SaveChangesAsync();
    }
}
