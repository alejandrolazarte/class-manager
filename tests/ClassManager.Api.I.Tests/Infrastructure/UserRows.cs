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

    public static Guid? UserIdOf(string accessToken) =>
        AuthenticationRequests.AccessTokenClaim(accessToken, SessionClaimTypes.UserId) is { } userId ? Guid.Parse(userId) : null;

    public static async Task<Guid?> FindUserIdByAccountAsync(this ApiFixture fixture, Guid businessId, Guid accountId)
    {
        await using var context = fixture.CreateDbContext(businessId);
        return await context.Users
            .Where(user => user.AccountId == accountId)
            .Select(user => (Guid?)user.Id)
            .SingleOrDefaultAsync();
    }

    public static async Task ChangeUserOfClientAsync(
        this ApiFixture fixture,
        Guid businessId,
        Guid clientId,
        string fullName,
        string? email = null,
        string? normalizedPhoneNumber = null)
    {
        await using var context = fixture.CreateDbContext(businessId);
        var userId = await context.Clients.Where(client => client.Id == clientId).Select(client => client.UserId).SingleAsync();
        await ChangeAsync(context, userId, fullName, email, normalizedPhoneNumber);
    }

    public static async Task ChangeUserOfStudentAsync(this ApiFixture fixture, Guid businessId, Guid studentId, string fullName, string? email = null)
    {
        await using var context = fixture.CreateDbContext(businessId);
        var userId = await context.Students.Where(student => student.Id == studentId).Select(student => student.UserId).SingleAsync();
        await ChangeAsync(context, userId, fullName, email, null);
    }

    public static async Task ChangeUserOfInstructorAsync(this ApiFixture fixture, Guid businessId, Guid instructorId, string fullName)
    {
        await using var context = fixture.CreateDbContext(businessId);
        var userId = await context.Instructors.Where(instructor => instructor.Id == instructorId).Select(instructor => instructor.UserId).SingleAsync();
        await ChangeAsync(context, userId, fullName, null, null);
    }

    public static async Task RenameUserAsync(this ApiFixture fixture, Guid businessId, Guid userId, string fullName)
    {
        await using var context = fixture.CreateDbContext(businessId);
        await ChangeAsync(context, userId, fullName, null, null);
    }

    private static async Task ChangeAsync(AppDbContext context, Guid? userId, string fullName, string? email, string? normalizedPhoneNumber)
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
