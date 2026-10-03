using ClassManager.Core.Abstractions.Email;
using ClassManager.Core.Abstractions.Notifications;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Domain.Accounts;
using ClassManager.Infrastructure.Email;
using ClassManager.Infrastructure.Notifications;
using ClassManager.Infrastructure.Persistence;
using ClassManager.Infrastructure.Persistence.Repositories;
using ClassManager.Infrastructure.Security;
using ClassManager.Infrastructure.WebPush;
using ClassManager.Notifications.Delivery.Hosting;
using ClassManager.Notifications.WebPush;
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
        services.AddScoped<IOrganizationRepository, OrganizationRepository>();
        services.AddScoped<IOrganizationMemberRepository, OrganizationMemberRepository>();
        services.AddScoped<IBusinessRepository, BusinessRepository>();
        services.AddScoped<IBrandLogoRepository, BrandLogoRepository>();
        services.AddScoped<IBusinessMemberRepository, BusinessMemberRepository>();
        services.AddScoped<IMemberInvitationRepository, MemberInvitationRepository>();
        services.AddScoped<ICustomRoleRepository, CustomRoleRepository>();
        services.AddScoped<IClientAccountRepository, ClientAccountRepository>();
        services.AddScoped<IClientInvitationRepository, ClientInvitationRepository>();
        services.AddScoped<IClientRepository, ClientRepository>();
        services.AddScoped<IStudentRepository, StudentRepository>();
        services.AddScoped<IInstructorRepository, InstructorRepository>();
        services.AddScoped<IClassGroupRepository, ClassGroupRepository>();
        services.AddScoped<IEnrollmentRepository, EnrollmentRepository>();
        services.AddScoped<IClassSessionRepository, ClassSessionRepository>();
        services.AddScoped<IAttendanceRepository, AttendanceRepository>();
        services.AddScoped<IClassFeedbackRepository, ClassFeedbackRepository>();
        services.AddScoped<IAnnouncementRepository, AnnouncementRepository>();
        services.AddScoped<IAbsenceNoticeRepository, AbsenceNoticeRepository>();
        services.AddScoped<IMakeupBookingRepository, MakeupBookingRepository>();
        services.AddScoped<IPackBookingRepository, PackBookingRepository>();
        services.AddScoped<IAchievementLevelRepository, AchievementLevelRepository>();
        services.AddScoped<IPushSubscriptionRepository, PushSubscriptionRepository>();
        services.AddScoped<IMemberPushSubscriptionRepository, MemberPushSubscriptionRepository>();
        services.AddScoped<ITeamNotificationRepository, TeamNotificationRepository>();
        services.AddScoped<IPaymentRepository, PaymentRepository>();
        services.AddScoped<IFeeScheduleRepository, FeeScheduleRepository>();
        services.AddScoped<IClassPackRepository, ClassPackRepository>();
        services.AddScoped<IClassPackPurchaseRepository, ClassPackPurchaseRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IStockMovementRepository, StockMovementRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IStockLock, StockLock>();
        services.AddScoped<IOrderNumbers, OrderNumbers>();
        services.AddScoped<IExpiredOrderDirectory, ExpiredOrderDirectory>();
        services.AddScoped<IOrderNotificationService, OrderNotificationService>();
        services.AddScoped<IPrivateLessonRepository, PrivateLessonRepository>();

        return services.AddEmail().AddWebPush().AddSecurity();
    }

    private static IServiceCollection AddEmail(this IServiceCollection services)
    {
        services.AddEmailTransport();
        services.AddSingleton<IWebAppLinks, WebAppLinks>();
        services.AddScoped<EmailBrandReader>();
        services.AddScoped<IEmailSender, BrandedEmailSender>();

        return services;
    }

    private static IServiceCollection AddWebPush(this IServiceCollection services)
    {
        services.AddWebPushSender();
        services.AddSingleton<PushOutbox>();
        services.AddSingleton<IWebPushKeyProvider, WebPushKeyProvider>();
        services.AddScoped<PushPublisher>();
        services.AddScoped(serviceProvider => new PushDispatcher(
            serviceProvider.GetRequiredService<AppDbContext>(), serviceProvider.GetRequiredService<IWebPushSender>()));
        services.AddScoped<IFamilyNotificationService, FamilyNotificationService>();
        services.AddScoped<TeamNotifier>();
        services.AddScoped<ITeamNotificationService, TeamNotificationService>();

        return services;
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
        services.AddScoped<IFamilyDirectory, FamilyDirectory>();
        services.AddScoped<IFamilyAccess, CurrentFamily>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IIdentityService, IdentityService>();
        services.AddScoped<ICurrentMember, CurrentMember>();
        services.AddScoped<IBranchDirectory, BranchDirectory>();
        services.AddSingleton<ISecretTokenGenerator, SecretTokenGenerator>();

        return services;
    }
}
