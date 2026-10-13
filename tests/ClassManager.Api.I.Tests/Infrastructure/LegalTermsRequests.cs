using ClassManager.Core.Domain.Accounts;
using Microsoft.EntityFrameworkCore;

namespace ClassManager.Api.I.Tests.Infrastructure;

public static class LegalTermsRequests
{
    public static async Task<List<LegalTermsAcceptance>> LegalTermsAcceptancesOfAsync(this ApiFixture fixture, string email)
    {
        await using var securityContext = fixture.CreateSecurityDbContext();
        var userId = await securityContext.Users.Where(user => user.Email == email).Select(user => user.Id).SingleAsync();
        await using var context = fixture.CreateDbContext(Guid.Empty);
        return await context.LegalTermsAcceptances.AsNoTracking().Where(acceptance => acceptance.UserId == userId).ToListAsync();
    }
}
