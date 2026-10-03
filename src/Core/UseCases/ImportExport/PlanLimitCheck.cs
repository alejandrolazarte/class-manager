using ClassManager.Subscriptions.Access;

namespace ClassManager.Core.UseCases.ImportExport;

public sealed record PlanLimitCheck(IFeatureAccess FeatureAccess, IFeatureUsage FeatureUsage);
