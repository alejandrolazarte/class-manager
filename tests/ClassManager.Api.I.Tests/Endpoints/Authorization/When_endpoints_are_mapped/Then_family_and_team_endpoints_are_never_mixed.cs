using ClassManager.Api.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace ClassManager.Api.I.Tests.Endpoints.Authorization.When_endpoints_are_mapped;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_family_and_team_endpoints_are_never_mixed(ApiFixture fixture)
{
    [Fact]
    public void Then_family_and_team_endpoints_are_never_mixed_Run()
    {
        var endpoints = fixture.ApiFactory.Services.GetRequiredService<EndpointDataSource>().Endpoints.OfType<RouteEndpoint>().ToList();

        var familyEndpointsWithoutFamilyPolicy = endpoints
            .Where(endpoint => IsUnderFamilyRoute(endpoint) && !HasOnlyFamilyPolicy(endpoint))
            .Select(endpoint => endpoint.DisplayName)
            .ToList();
        var familyPolicyOutsideFamilyRoute = endpoints
            .Where(endpoint => !IsUnderFamilyRoute(endpoint) && PoliciesOf(endpoint).Contains(AuthorizationPolicies.Family))
            .Select(endpoint => endpoint.DisplayName)
            .ToList();

        endpoints.Any(IsUnderFamilyRoute).ShouldBeTrue();
        familyEndpointsWithoutFamilyPolicy.ShouldBeEmpty();
        familyPolicyOutsideFamilyRoute.ShouldBeEmpty();
    }

    private static bool IsUnderFamilyRoute(RouteEndpoint endpoint) =>
        endpoint.RoutePattern.RawText?.StartsWith(ApiRoutes.Family, StringComparison.Ordinal) == true;

    private static bool HasOnlyFamilyPolicy(Endpoint endpoint)
    {
        var policies = PoliciesOf(endpoint);
        return policies.Count > 0 && policies.All(policy => policy == AuthorizationPolicies.Family);
    }

    private static List<string?> PoliciesOf(Endpoint endpoint) =>
        [.. endpoint.Metadata.GetOrderedMetadata<IAuthorizeData>().Select(authorizeData => authorizeData.Policy)];
}
