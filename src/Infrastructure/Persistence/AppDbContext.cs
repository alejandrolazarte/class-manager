using ClassManager.Tenancy.AspNetCore.Persistence;

namespace ClassManager.Infrastructure.Persistence;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options, ITenantContext tenantContext) : DbContext(options), ITenantDbContext
{
    public DbSet<Business> Businesses => Set<Business>();
    public DbSet<BusinessMember> BusinessMembers => Set<BusinessMember>();
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

    public Guid CurrentTenantId => tenantContext.TenantId;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(AppDbContext).Assembly,
            configurationType => configurationType.Namespace == typeof(Configurations.BusinessConfiguration).Namespace);
        modelBuilder.ApplyTenantQueryFilters(this);
    }
}
