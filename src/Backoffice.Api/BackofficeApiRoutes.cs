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
    public const string WebAppSignIn = "/sign-in";
    public const string AccessRequestCodeParameter = "accessRequestCode";
}
