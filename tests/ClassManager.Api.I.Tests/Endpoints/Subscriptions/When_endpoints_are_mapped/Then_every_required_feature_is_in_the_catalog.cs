using ClassManager.Core.Domain.Subscriptions;
using ClassManager.Subscriptions.AspNetCore.Endpoints;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace ClassManager.Api.I.Tests.Endpoints.Subscriptions.When_endpoints_are_mapped;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_every_required_feature_is_in_the_catalog(ApiFixture fixture)
{
    [Fact]
    public void Then_every_required_feature_is_in_the_catalog_Run()
    {
        var requiredFeatureCodes = fixture.ApiFactory.Services.GetRequiredService<EndpointDataSource>().Endpoints
            .SelectMany(endpoint => endpoint.Metadata.GetOrderedMetadata<RequiredFeatureMetadata>())
            .Select(metadata => metadata.FeatureCode)
            .Distinct()
            .ToList();

        requiredFeatureCodes.ShouldNotBeEmpty();
        requiredFeatureCodes.ShouldAllBe(featureCode => Features.All.Contains(featureCode));
    }
}
