using ClassManager.Infrastructure.BackgroundTasks;
using ClassManager.Subscriptions.AspNetCore.Persistence;
using ClassManager.Subscriptions.Catalog;
using ClassManager.Subscriptions.Subscribers;
using ClassManager.Tenancy.AspNetCore.Persistence;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace ClassManager.Infrastructure.Persistence;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options, ITenantContext tenantContext) : DbContext(options), ITenantDbContext, ISubscriptionsDbContext
{
    public DbSet<Organization> Organizations => Set<Organization>();
    public DbSet<OrganizationMember> OrganizationMembers => Set<OrganizationMember>();
    public DbSet<Business> Businesses => Set<Business>();
    public DbSet<BrandLogo> BrandLogos => Set<BrandLogo>();
    public DbSet<BusinessMember> BusinessMembers => Set<BusinessMember>();
    public DbSet<MemberInvitation> MemberInvitations => Set<MemberInvitation>();
    public DbSet<CustomRole> CustomRoles => Set<CustomRole>();
    public DbSet<ClientAccount> ClientAccounts => Set<ClientAccount>();
    public DbSet<ClientInvitation> ClientInvitations => Set<ClientInvitation>();
    public DbSet<Client> Clients => Set<Client>();
    public DbSet<Student> Students => Set<Student>();
    public DbSet<Instructor> Instructors => Set<Instructor>();
    public DbSet<ClassGroup> ClassGroups => Set<ClassGroup>();
    public DbSet<Enrollment> Enrollments => Set<Enrollment>();
    public DbSet<ClassSession> ClassSessions => Set<ClassSession>();
    public DbSet<Attendance> Attendances => Set<Attendance>();
    public DbSet<ClassFeedback> ClassFeedbacks => Set<ClassFeedback>();
    public DbSet<Announcement> Announcements => Set<Announcement>();
    public DbSet<AbsenceNotice> AbsenceNotices => Set<AbsenceNotice>();
    public DbSet<MakeupBooking> MakeupBookings => Set<MakeupBooking>();
    public DbSet<PackBooking> PackBookings => Set<PackBooking>();
    public DbSet<AchievementLevel> AchievementLevels => Set<AchievementLevel>();
    public DbSet<PushSubscription> PushSubscriptions => Set<PushSubscription>();
    public DbSet<MemberPushSubscription> MemberPushSubscriptions => Set<MemberPushSubscription>();
    public DbSet<TeamNotification> TeamNotifications => Set<TeamNotification>();
    public DbSet<BackgroundTask> BackgroundTasks => Set<BackgroundTask>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<DefaultMonthlyFeeChange> DefaultMonthlyFeeChanges => Set<DefaultMonthlyFeeChange>();
    public DbSet<ClientBillingPlanChange> ClientBillingPlanChanges => Set<ClientBillingPlanChange>();
    public DbSet<ClassPack> ClassPacks => Set<ClassPack>();
    public DbSet<ClassPackImage> ClassPackImages => Set<ClassPackImage>();
    public DbSet<ClassPackPurchase> ClassPackPurchases => Set<ClassPackPurchase>();
    public DbSet<ClassPackClassGroup> ClassPackClassGroups => Set<ClassPackClassGroup>();
    public DbSet<PrivateLesson> PrivateLessons => Set<PrivateLesson>();
    public DbSet<PrivateLessonStudent> PrivateLessonStudents => Set<PrivateLessonStudent>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductVariant> ProductVariants => Set<ProductVariant>();
    public DbSet<ProductImage> ProductImages => Set<ProductImage>();
    public DbSet<Document> Documents => Set<Document>();
    public DbSet<StockMovement> StockMovements => Set<StockMovement>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderLine> OrderLines => Set<OrderLine>();
    public DbSet<Plan> Plans => Set<Plan>();
    public DbSet<Feature> Features => Set<Feature>();
    public DbSet<PlanFeature> PlanFeatures => Set<PlanFeature>();
    public DbSet<Subscription> Subscriptions => Set<Subscription>();
    public DbSet<SubscriptionFeature> SubscriptionFeatures => Set<SubscriptionFeature>();

    public Guid CurrentTenantId => tenantContext.TenantId;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
        optionsBuilder.ConfigureWarnings(warnings => warnings.Throw(CoreEventId.PossibleIncorrectRequiredNavigationWithQueryFilterInteractionWarning));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(AppDbContext).Assembly,
            configurationType => configurationType.Namespace == typeof(Configurations.BusinessConfiguration).Namespace);
        modelBuilder.ApplySubscriptionsModel();
        modelBuilder.Entity<Subscription>().HasOne<Organization>().WithMany().HasForeignKey(subscription => subscription.SubscriberId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.SeedSubscriptionCatalog();
        modelBuilder.ApplyTenantQueryFilters(this);
        modelBuilder.ApplySoftDeleteQueryFilters();
    }
}
