namespace ClassManager.Backoffice.Api;

internal static class BackofficeApiRoutes
{
    public const string Health = "/health";
    public const string Authentication = "/api/auth";
    public const string GoogleSignIn = "/signin/google";
    public const string GoogleSignInComplete = "/signin/google/complete";
    public const string Refresh = "/refresh";
    public const string SignOut = "/signout";
    public const string CurrentOperator = "/api/me";
    public const string Brands = "/api/brands";
    public const string BrandById = "/{brandId:guid}";
    public const string BrandSubscription = "/{brandId:guid}/subscription";
    public const string BrandAddOns = "/{brandId:guid}/add-ons";
    public const string BrandAddOnById = "/{brandId:guid}/add-ons/{addOnId:guid}";
    public const string BrandAddOnRestore = "/{brandId:guid}/add-ons/{addOnId:guid}/restore";
    public const string SubscriptionCatalog = "/api/subscription-catalog";
    public const string ExpiringAddOns = "/api/expiring-add-ons";
    public const string Operators = "/api/operators";
    public const string OperatorById = "/{loginId:guid}";
    public const string CommandLog = "/api/logs/commands";
    public const string SecurityEvents = "/api/logs/security-events";
    public const string WebAppSignIn = "/sign-in";
    public const string AccessRequestCodeParameter = "accessRequestCode";
}
