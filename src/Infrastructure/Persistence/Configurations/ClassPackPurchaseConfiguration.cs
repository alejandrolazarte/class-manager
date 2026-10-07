using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClassManager.Infrastructure.Persistence.Configurations;

internal sealed class ClassPackPurchaseConfiguration : IEntityTypeConfiguration<ClassPackPurchase>
{
    private const int MethodMaxLength = 16;

    public void Configure(EntityTypeBuilder<ClassPackPurchase> builder)
    {
        builder.HasKey(purchase => purchase.Id);
        builder.Property(purchase => purchase.Id).ValueGeneratedNever();
        builder.Property(purchase => purchase.Name).HasMaxLength(ClassPack.NameMaxLength).IsRequired();
        builder.Property(purchase => purchase.Price).HasPrecision(MonthlyFee.AmountPrecision, MonthlyFee.AmountDecimals);
        builder.HasOne<PrivateLesson>().WithMany().HasForeignKey(purchase => purchase.TrialLessonId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(purchase => new { purchase.TenantId, purchase.TrialLessonId })
            .IsUnique()
            .HasFilter($"[TrialLessonId] IS NOT NULL AND {SoftDeleteModelBuilderExtensions.NotDeletedFilter}");
        builder.Property(purchase => purchase.Method).HasConversion<string>().HasMaxLength(MethodMaxLength);
        builder.Property(purchase => purchase.Notes).HasMaxLength(ClassPackPurchase.NotesMaxLength);

        builder.HasOne<Business>().WithMany().HasForeignKey(purchase => purchase.TenantId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Client>().WithMany().HasForeignKey(purchase => purchase.ClientId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<ClassPack>().WithMany().HasForeignKey(purchase => purchase.ClassPackId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(purchase => new { purchase.TenantId, purchase.ClientId });
        builder.HasIndex(purchase => new { purchase.TenantId, purchase.PurchasedOn });
    }
}
