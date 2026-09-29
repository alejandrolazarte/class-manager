using ClassManager.Tenancy.AspNetCore.Persistence;

namespace ClassManager.Infrastructure.Persistence;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options, ITenantContext tenantContext) : DbContext(options), ITenantDbContext
{
    public DbSet<Organization> Organizations => Set<Organization>();
    public DbSet<OrganizationMember> OrganizationMembers => Set<OrganizationMember>();
    public DbSet<Business> Businesses => Set<Business>();
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
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<DefaultMonthlyFeeChange> DefaultMonthlyFeeChanges => Set<DefaultMonthlyFeeChange>();
    public DbSet<ClientBillingPlanChange> ClientBillingPlanChanges => Set<ClientBillingPlanChange>();
    public DbSet<ClassPack> ClassPacks => Set<ClassPack>();
    public DbSet<ClassPackPurchase> ClassPackPurchases => Set<ClassPackPurchase>();
    public DbSet<PrivateLesson> PrivateLessons => Set<PrivateLesson>();
    public DbSet<PrivateLessonStudent> PrivateLessonStudents => Set<PrivateLessonStudent>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductVariant> ProductVariants => Set<ProductVariant>();
    public DbSet<StockMovement> StockMovements => Set<StockMovement>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderLine> OrderLines => Set<OrderLine>();

    public Guid CurrentTenantId => tenantContext.TenantId;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(AppDbContext).Assembly,
            configurationType => configurationType.Namespace == typeof(Configurations.BusinessConfiguration).Namespace);
        modelBuilder.ApplyTenantQueryFilters(this);
    }
}
