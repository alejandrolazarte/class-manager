using ClassManager.Backoffice.Core.Operators;
using ClassManager.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace ClassManager.Backoffice.Infrastructure.Persistence;

public sealed class BackofficeDbContext(DbContextOptions<BackofficeDbContext> options) : DbContext(options), ISharedTransactionContext
{
    public const string SchemaName = "backoffice";
    public const string MigrationsHistoryTableName = "__EFMigrationsHistory";

    public DbSet<BackofficeLogin> BackofficeLogins => Set<BackofficeLogin>();

    public DbSet<BackofficeAccessRequest> BackofficeAccessRequests => Set<BackofficeAccessRequest>();

    public static void ConfigureSqlServer(SqlServerDbContextOptionsBuilder sqlServer) =>
        sqlServer.MigrationsHistoryTable(MigrationsHistoryTableName, SchemaName);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(SchemaName);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BackofficeDbContext).Assembly);
    }
}
