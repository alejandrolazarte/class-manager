namespace ClassManager.Core.Domain.Subscriptions;

public static class PlanCodes
{
    public const string Free = "free";
    public const string Lite = "lite";
    public const string Pro = "pro";
    public const string Enterprise = "enterprise";

    public static IReadOnlyList<string> All { get; } = [Free, Lite, Pro, Enterprise];
}
