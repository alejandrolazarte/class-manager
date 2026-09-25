using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ClassManager.Security.Persistence;

internal sealed class DesignTimeSecurityDbContextFactory : IDesignTimeDbContextFactory<SecurityDbContext>
{
    private const string DesignTimeConnectionString = "Server=localhost;Database=SecurityDesignTime;Trusted_Connection=True;TrustServerCertificate=True";

    public SecurityDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<SecurityDbContext>()
            .UseSqlServer(DesignTimeConnectionString, SecurityDbContext.ConfigureSqlServer)
            .Options;

        return new SecurityDbContext(options);
    }
}
