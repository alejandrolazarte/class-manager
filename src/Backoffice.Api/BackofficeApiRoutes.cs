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
    public const string BrandById = "/{brandId:long}";
    public const string BrandSubscription = "/{brandId:long}/subscription";
    public const string BrandAddOns = "/{brandId:long}/add-ons";
    public const string BrandAddOnById = "/{brandId:long}/add-ons/{addOnId:long}";
    public const string BrandAddOnRestore = "/{brandId:long}/add-ons/{addOnId:long}/restore";
    public const string SubscriptionCatalog = "/api/subscription-catalog";
    public const string ExpiringAddOns = "/api/expiring-add-ons";
    public const string Operators = "/api/operators";
    public const string OperatorById = "/{loginId:long}";
    public const string CommandLog = "/api/logs/commands";
    public const string SecurityEvents = "/api/logs/security-events";
    public const string WebAppSignIn = "/sign-in";
    public const string AccessRequestCodeParameter = "accessRequestCode";
}
