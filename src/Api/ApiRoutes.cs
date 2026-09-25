namespace ClassManager.Api;

public static class ApiRoutes
{
    public const string Health = "/health";
    public const string Clients = "/api/clients";
    public const string ClientById = "/{clientId:guid}";
    public const string Business = "/api/business";
    public const string Authentication = "/api/auth";
    public const string SignUp = "/sign-up";
    public const string SignIn = "/sign-in";
    public const string Refresh = "/refresh";
    public const string SignOut = "/sign-out";
}
