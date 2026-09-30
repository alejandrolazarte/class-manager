using ClassManager.Api.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace ClassManager.Api.I.Tests.Endpoints.Authorization.When_endpoints_are_mapped;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_every_endpoint_requires_a_permission_or_is_anonymous(ApiFixture fixture)
{
    [Fact]
    public void Then_every_endpoint_requires_a_permission_or_is_anonymous_Run()
    {
        var endpoints = fixture.ApiFactory.Services.GetRequiredService<EndpointDataSource>().Endpoints.OfType<RouteEndpoint>().ToList();

        var unprotectedEndpoints = endpoints
            .Where(endpoint => !IsAnonymous(endpoint) && !RequiresPermission(endpoint) && !RequiresFamily(endpoint) && !RequiresAnyAccount(endpoint))
            .Select(endpoint => endpoint.DisplayName)
            .ToList();

        endpoints.ShouldNotBeEmpty();
        unprotectedEndpoints.ShouldBeEmpty();
    }

    private static bool IsAnonymous(Endpoint endpoint) => endpoint.Metadata.GetMetadata<IAllowAnonymous>() is not null;

    private static bool RequiresPermission(Endpoint endpoint) =>
        endpoint.Metadata.GetOrderedMetadata<IAuthorizeData>()
            .Any(authorizeData => authorizeData.Policy?.StartsWith(AuthorizationPolicies.PermissionPrefix, StringComparison.Ordinal) == true);

    private static bool RequiresAnyAccount(Endpoint endpoint) =>
        endpoint.Metadata.GetOrderedMetadata<IAuthorizeData>()
            .Any(authorizeData => authorizeData.Policy == AuthorizationPolicies.AnyAccount);

    private static bool RequiresFamily(Endpoint endpoint) =>
        endpoint.Metadata.GetOrderedMetadata<IAuthorizeData>()
            .Any(authorizeData => authorizeData.Policy == AuthorizationPolicies.Family);
}
