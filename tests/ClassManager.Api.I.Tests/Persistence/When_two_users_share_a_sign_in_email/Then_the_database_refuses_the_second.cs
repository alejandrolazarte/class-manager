using ClassManager.Security.Accounts;
using Microsoft.EntityFrameworkCore;

namespace ClassManager.Api.I.Tests.Persistence.When_two_users_share_a_sign_in_email;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_database_refuses_the_second(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_database_refuses_the_second_Run()
    {
        var normalizedEmail = AuthenticationRequests.UniqueEmail().ToUpperInvariant();
        await using var securityContext = fixture.CreateSecurityDbContext();
        securityContext.Users.Add(UserWith(normalizedEmail, "FIRST"));
        securityContext.Users.Add(UserWith(normalizedEmail, "SECOND"));

        var save = () => securityContext.SaveChangesAsync();

        await save.ShouldThrowAsync<DbUpdateException>();
    }

    private static ApplicationUser UserWith(string normalizedEmail, string userNamePrefix) =>
        new()
        {
            Id = Guid.CreateVersion7(),
            Email = normalizedEmail,
            NormalizedEmail = normalizedEmail,
            UserName = $"{userNamePrefix}-{normalizedEmail}",
            NormalizedUserName = $"{userNamePrefix}-{normalizedEmail}",
            FullName = "Ana Pérez",
            SecurityStamp = Guid.NewGuid().ToString(),
        };
}
