using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ClassManager.Backoffice.Infrastructure.Persistence;

internal sealed class DesignTimeBackofficeDbContextFactory : IDesignTimeDbContextFactory<BackofficeDbContext>
{
    private const string DesignTimeConnectionString = "Server=localhost;Database=BackofficeDesignTime;Trusted_Connection=True;TrustServerCertificate=True";

    public BackofficeDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<BackofficeDbContext>()
            .UseSqlServer(DesignTimeConnectionString, BackofficeDbContext.ConfigureSqlServer)
            .Options;

        return new BackofficeDbContext(options);
    }
}
