using ClassManager.Subscriptions.Billing;
using ClassManager.Subscriptions.Catalog;
using ClassManager.Subscriptions.Offers;
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
    public const string ProcessedBillingEventsTable = "ProcessedBillingEvents";
    public const string BillingCustomersTable = "BillingCustomers";
    public const string PlanOffersTable = "PlanOffers";

    private const string NotDeletedFilter = "[IsDeleted] = 0";
    private const string CurrentExternalSubscriptionFilter = "[IsDeleted] = 0 AND [ExternalSubscriptionId] IS NOT NULL";
    private const int BillingPeriodMaxLength = 20;
    private const int PaymentStatusMaxLength = 20;

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
            subscription.Property(entity => entity.PlanCode).HasMaxLength(CatalogRules.CodeMaxLength).IsRequired();
            subscription.Property(entity => entity.Price).HasPrecision(CatalogRules.PricePrecision, CatalogRules.PriceScale);
            subscription.Property(entity => entity.Currency).HasMaxLength(CatalogRules.CurrencyLength).IsFixedLength().IsRequired();
            subscription.Property(entity => entity.Note).HasMaxLength(CatalogRules.NoteMaxLength);
            subscription.HasOne<Plan>().WithMany().HasForeignKey(entity => entity.PlanCode).OnDelete(DeleteBehavior.Restrict);
            subscription.Property(entity => entity.ExternalSubscriptionId).HasMaxLength(Subscription.ExternalSubscriptionIdMaxLength);
            subscription.Property(entity => entity.PaymentStatus).HasConversion<string>().HasMaxLength(PaymentStatusMaxLength);
            subscription.HasIndex(entity => entity.SubscriberId).IsUnique().HasFilter(NotDeletedFilter);
            subscription.HasIndex(entity => entity.ExternalSubscriptionId).IsUnique().HasFilter(CurrentExternalSubscriptionFilter);
        });

        modelBuilder.Entity<SubscriptionFeature>(subscriptionFeature =>
        {
            subscriptionFeature.ToTable(SubscriptionFeaturesTable, schema);
            subscriptionFeature.HasKey(entity => entity.Id);
            subscriptionFeature.Property(entity => entity.FeatureCode).HasMaxLength(CatalogRules.CodeMaxLength).IsRequired();
            subscriptionFeature.Property(entity => entity.Price).HasPrecision(CatalogRules.PricePrecision, CatalogRules.PriceScale);
            subscriptionFeature.Property(entity => entity.Currency).HasMaxLength(CatalogRules.CurrencyLength).IsFixedLength().IsRequired();
            subscriptionFeature.HasOne<Subscription>().WithMany().HasForeignKey(entity => entity.SubscriptionId).OnDelete(DeleteBehavior.Cascade);
            subscriptionFeature.HasOne<Feature>().WithMany().HasForeignKey(entity => entity.FeatureCode).OnDelete(DeleteBehavior.Restrict);
            subscriptionFeature.HasIndex(entity => new { entity.SubscriptionId, entity.FeatureCode }).IsUnique().HasFilter(NotDeletedFilter);
        });

        modelBuilder.Entity<ProcessedBillingEvent>(processedEvent =>
        {
            processedEvent.ToTable(ProcessedBillingEventsTable, schema);
            processedEvent.HasKey(entity => entity.EventId);
            processedEvent.Property(entity => entity.EventId).HasMaxLength(ProcessedBillingEvent.EventIdMaxLength);
        });

        modelBuilder.Entity<BillingCustomer>(billingCustomer =>
        {
            billingCustomer.ToTable(BillingCustomersTable, schema);
            billingCustomer.HasKey(entity => entity.SubscriberId);
            billingCustomer.Property(entity => entity.SubscriberId).ValueGeneratedNever();
            billingCustomer.Property(entity => entity.ExternalCustomerId).HasMaxLength(BillingCustomer.ExternalCustomerIdMaxLength).IsRequired();
            billingCustomer.HasIndex(entity => entity.ExternalCustomerId).IsUnique();
        });

        modelBuilder.Entity<PlanOffer>(planOffer =>
        {
            planOffer.ToTable(PlanOffersTable, schema);
            planOffer.HasKey(entity => entity.Id);
            planOffer.Property(entity => entity.PlanCode).HasMaxLength(CatalogRules.CodeMaxLength).IsRequired();
            planOffer.Property(entity => entity.Price).HasPrecision(CatalogRules.PricePrecision, CatalogRules.PriceScale);
            planOffer.Property(entity => entity.Currency).HasMaxLength(CatalogRules.CurrencyLength).IsFixedLength().IsRequired();
            planOffer.Property(entity => entity.Note).HasMaxLength(CatalogRules.NoteMaxLength);
            planOffer.HasOne<Plan>().WithMany().HasForeignKey(entity => entity.PlanCode).OnDelete(DeleteBehavior.Restrict);
            planOffer.HasIndex(entity => entity.SubscriberId).IsUnique().HasFilter(NotDeletedFilter);
        });

        return modelBuilder;
    }
}
