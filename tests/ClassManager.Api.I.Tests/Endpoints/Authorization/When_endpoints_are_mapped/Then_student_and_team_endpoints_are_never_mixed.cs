using ClassManager.Api.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace ClassManager.Api.I.Tests.Endpoints.Authorization.When_endpoints_are_mapped;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_student_and_team_endpoints_are_never_mixed(ApiFixture fixture)
{
    [Fact]
    public void Then_student_and_team_endpoints_are_never_mixed_Run()
    {
        var endpoints = fixture.ApiFactory.Services.GetRequiredService<EndpointDataSource>().Endpoints.OfType<RouteEndpoint>().ToList();

        var studentAppEndpointsWithoutStudentAppPolicy = endpoints
            .Where(endpoint => IsUnderStudentAppRoute(endpoint) && !HasOnlyStudentPolicy(endpoint))
            .Select(endpoint => endpoint.DisplayName)
            .ToList();
        var studentAppPolicyOutsideStudentAppRoute = endpoints
            .Where(endpoint => !IsUnderStudentAppRoute(endpoint) && PoliciesOf(endpoint).Contains(AuthorizationPolicies.Student))
            .Select(endpoint => endpoint.DisplayName)
            .ToList();

        endpoints.Any(IsUnderStudentAppRoute).ShouldBeTrue();
        studentAppEndpointsWithoutStudentAppPolicy.ShouldBeEmpty();
        studentAppPolicyOutsideStudentAppRoute.ShouldBeEmpty();
    }

    private static bool IsUnderStudentAppRoute(RouteEndpoint endpoint) =>
        endpoint.RoutePattern.RawText?.StartsWith(ApiRoutes.StudentApp, StringComparison.Ordinal) == true;

    private static bool HasOnlyStudentPolicy(Endpoint endpoint)
    {
        var policies = PoliciesOf(endpoint);
        return policies.Count > 0 && policies.All(policy => policy == AuthorizationPolicies.Student);
    }

    private static List<string?> PoliciesOf(Endpoint endpoint) =>
        [.. endpoint.Metadata.GetOrderedMetadata<IAuthorizeData>().Select(authorizeData => authorizeData.Policy)];
}
