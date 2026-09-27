using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Domain.Accounts;
using ClassManager.Infrastructure.Persistence;
using ClassManager.Infrastructure.Persistence.Repositories;
using ClassManager.Infrastructure.Security;
using ClassManager.Security.Hosting;
using ClassManager.Security.Persistence;
using ClassManager.Security.Tokens;
using ClassManager.Tenancy.AspNetCore.Hosting;
using ClassManager.Tenancy.AspNetCore.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ClassManager.Infrastructure;

public static class InfrastructureServiceCollectionExtensions
{
    public const string DatabaseConnectionStringName = "BusinessDatabase";
    public const int MaximumFailedSignInAttempts = 5;

    private const string MissingConnectionStringMessage = "Connection string 'BusinessDatabase' is not configured.";

    private static readonly TimeSpan LockoutDuration = TimeSpan.FromMinutes(15);

    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddTenantStamping();
        services.AddDbContext<AppDbContext>((serviceProvider, options) =>
        {
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();
            var connectionString = configuration.GetConnectionString(DatabaseConnectionStringName)
                ?? throw new InvalidOperationException(MissingConnectionStringMessage);
            options
                .UseSqlServer(connectionString)
                .AddInterceptors(serviceProvider.GetRequiredService<TenantStampingSaveChangesInterceptor>());
        });

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IBusinessRepository, BusinessRepository>();
        services.AddScoped<IBusinessMemberRepository, BusinessMemberRepository>();
        services.AddScoped<IClientRepository, ClientRepository>();
        services.AddScoped<IStudentRepository, StudentRepository>();
        services.AddScoped<IInstructorRepository, InstructorRepository>();
        services.AddScoped<IClassGroupRepository, ClassGroupRepository>();
        services.AddScoped<IEnrollmentRepository, EnrollmentRepository>();
        services.AddScoped<IClassSessionRepository, ClassSessionRepository>();
        services.AddScoped<IAttendanceRepository, AttendanceRepository>();
        services.AddScoped<IPaymentRepository, PaymentRepository>();
        services.AddScoped<IFeeScheduleRepository, FeeScheduleRepository>();
        services.AddScoped<IClassPackRepository, ClassPackRepository>();
        services.AddScoped<IClassPackPurchaseRepository, ClassPackPurchaseRepository>();
        services.AddScoped<IPrivateLessonRepository, PrivateLessonRepository>();

        return services.AddSecurity();
    }

    private static IServiceCollection AddSecurity(this IServiceCollection services)
    {
        services.AddSecurityServices(security =>
        {
            security.TenantClaimType = TenantClaimTypes.TenantId;
            security.PasswordMinLength = OwnerAccount.PasswordMinLength;
            security.MaxFailedSignInAttempts = MaximumFailedSignInAttempts;
            security.LockoutDuration = LockoutDuration;
            security.ConfigureDbContext = (serviceProvider, options) =>
                options.UseSqlServer(
                    serviceProvider.GetRequiredService<AppDbContext>().Database.GetDbConnection(),
                    SecurityDbContext.ConfigureSqlServer);
        });

        services.AddScoped<ITokenSubjectResolver, BusinessTokenSubjectResolver>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IIdentityService, IdentityService>();

        return services;
    }
}
