using ClassManager.Subscriptions.Catalog;
using ClassManager.Subscriptions.Subscribers;
using Microsoft.EntityFrameworkCore;

namespace ClassManager.Subscriptions.AspNetCore.Persistence;

public static class SubscriptionsModelBuilderExtensions
{
    public const string DefaultSchema = "billing";
    public const string PlansTable = "Plans";
    public const string FeaturesTable = "Features";
    public const string PlanFeaturesTable = "PlanFeatures";
    public const string SubscriptionsTable = "Subscriptions";
    public const string SubscriptionFeaturesTable = "SubscriptionFeatures";

    private const string OpenEndedFilter = "[EndsOn] IS NULL";
    private const int BillingPeriodMaxLength = 20;

    public static ModelBuilder ApplySubscriptionsModel(this ModelBuilder modelBuilder, string schema = DefaultSchema)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        modelBuilder.Entity<Plan>(plan =>
        {
            plan.ToTable(PlansTable, schema);
            plan.HasKey(entity => entity.Code);
            plan.Property(entity => entity.Code).HasMaxLength(CatalogRules.CodeMaxLength);
            plan.Property(entity => entity.ListPrice).HasPrecision(CatalogRules.PricePrecision, CatalogRules.PriceScale);
            plan.Property(entity => entity.Currency).HasMaxLength(CatalogRules.CurrencyLength).IsFixedLength().IsRequired();
            plan.Property(entity => entity.BillingPeriod).HasConversion<string>().HasMaxLength(BillingPeriodMaxLength).IsRequired();
        });

        modelBuilder.Entity<Feature>(feature =>
        {
            feature.ToTable(FeaturesTable, schema);
            feature.HasKey(entity => entity.Code);
            feature.Property(entity => entity.Code).HasMaxLength(CatalogRules.CodeMaxLength);
            feature.Property(entity => entity.AddOnListPrice).HasPrecision(CatalogRules.PricePrecision, CatalogRules.PriceScale);
            feature.Property(entity => entity.Currency).HasMaxLength(CatalogRules.CurrencyLength).IsFixedLength();
        });

        modelBuilder.Entity<PlanFeature>(planFeature =>
        {
            planFeature.ToTable(PlanFeaturesTable, schema);
            planFeature.HasKey(entity => new { entity.PlanCode, entity.FeatureCode });
            planFeature.Property(entity => entity.PlanCode).HasMaxLength(CatalogRules.CodeMaxLength);
            planFeature.Property(entity => entity.FeatureCode).HasMaxLength(CatalogRules.CodeMaxLength);
            planFeature.HasOne<Plan>().WithMany().HasForeignKey(entity => entity.PlanCode).OnDelete(DeleteBehavior.Cascade);
            planFeature.HasOne<Feature>().WithMany().HasForeignKey(entity => entity.FeatureCode).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Subscription>(subscription =>
        {
            subscription.ToTable(SubscriptionsTable, schema);
            subscription.HasKey(entity => entity.Id);
            subscription.Property(entity => entity.Id).ValueGeneratedNever();
            subscription.Property(entity => entity.PlanCode).HasMaxLength(CatalogRules.CodeMaxLength).IsRequired();
            subscription.Property(entity => entity.Price).HasPrecision(CatalogRules.PricePrecision, CatalogRules.PriceScale);
            subscription.Property(entity => entity.Currency).HasMaxLength(CatalogRules.CurrencyLength).IsFixedLength().IsRequired();
            subscription.Property(entity => entity.Note).HasMaxLength(CatalogRules.NoteMaxLength);
            subscription.HasOne<Plan>().WithMany().HasForeignKey(entity => entity.PlanCode).OnDelete(DeleteBehavior.Restrict);
            subscription.HasIndex(entity => entity.SubscriberId).IsUnique().HasFilter(OpenEndedFilter);
            subscription.HasIndex(entity => new { entity.SubscriberId, entity.StartsOn });
        });

        modelBuilder.Entity<SubscriptionFeature>(subscriptionFeature =>
        {
            subscriptionFeature.ToTable(SubscriptionFeaturesTable, schema);
            subscriptionFeature.HasKey(entity => entity.Id);
            subscriptionFeature.Property(entity => entity.Id).ValueGeneratedNever();
            subscriptionFeature.Property(entity => entity.FeatureCode).HasMaxLength(CatalogRules.CodeMaxLength).IsRequired();
            subscriptionFeature.Property(entity => entity.Price).HasPrecision(CatalogRules.PricePrecision, CatalogRules.PriceScale);
            subscriptionFeature.Property(entity => entity.Currency).HasMaxLength(CatalogRules.CurrencyLength).IsFixedLength().IsRequired();
            subscriptionFeature.HasOne<Subscription>().WithMany().HasForeignKey(entity => entity.SubscriptionId).OnDelete(DeleteBehavior.Cascade);
            subscriptionFeature.HasOne<Feature>().WithMany().HasForeignKey(entity => entity.FeatureCode).OnDelete(DeleteBehavior.Restrict);
            subscriptionFeature.HasIndex(entity => entity.SubscriptionId);
        });

        return modelBuilder;
    }
}
