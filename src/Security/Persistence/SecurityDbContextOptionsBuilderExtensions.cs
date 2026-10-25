using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ClassManager.Security.Persistence;

public static class SecurityDbContextOptionsBuilderExtensions
{
    private static readonly IServiceProvider IdentitySchemaServices = new ServiceCollection()
        .Configure<IdentityOptions>(identityOptions => identityOptions.Stores.SchemaVersion = SecurityDbContext.IdentitySchemaVersion)
        .BuildServiceProvider();

    /// <summary>
    /// Identity reads its schema version from the application's <see cref="IdentityOptions"/>. A
    /// <see cref="SecurityDbContext"/> created outside the application (design time, test fixtures) uses this to build
    /// the same model, passkeys included.
    /// </summary>
    public static DbContextOptionsBuilder<SecurityDbContext> UseIdentitySchema(this DbContextOptionsBuilder<SecurityDbContext> builder) =>
        builder.UseApplicationServiceProvider(IdentitySchemaServices);
}
