namespace ClassManager.Api.Authentication;

internal static class PermissionEndpointConventionBuilderExtensions
{
    public static TBuilder RequirePermission<TBuilder>(this TBuilder builder, string permission, params string[] alternativePermissions)
        where TBuilder : IEndpointConventionBuilder =>
        builder.RequireAuthorization(AuthorizationPolicies.AnyOf([permission, .. alternativePermissions]));

    public static TBuilder RequireStudent<TBuilder>(this TBuilder builder)
        where TBuilder : IEndpointConventionBuilder =>
        builder.RequireAuthorization(AuthorizationPolicies.Student);

    public static TBuilder RequireAnyAccount<TBuilder>(this TBuilder builder)
        where TBuilder : IEndpointConventionBuilder =>
        builder.RequireAuthorization(AuthorizationPolicies.AnyAccount);

    public static TBuilder RequireMember<TBuilder>(this TBuilder builder)
        where TBuilder : IEndpointConventionBuilder =>
        builder.RequireAuthorization(AuthorizationPolicies.Member);
}
