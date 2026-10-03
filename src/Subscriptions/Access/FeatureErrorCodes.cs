namespace ClassManager.Subscriptions.Access;

public static class FeatureErrorCodes
{
    public const string NotInPlan = "feature.not_in_plan";
    public const string NotInPlanMessage = "Your plan doesn't include this feature.";
    public const string LimitReached = "feature.limit_reached";
    public const string LimitReachedMessage = "Your plan's limit for this feature has been reached.";
    public const string FeatureDetail = "feature";
}
