using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Domain.Businesses;
using ClassManager.Infrastructure.Persistence.Repositories;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ClassManager.Api.I.Tests.Endpoints.Authentication.When_signing_up_and_business_creation_fails;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_no_user_is_left_behind(ApiFixture fixture)
{
    [Fact]
    public async Task Then_no_user_is_left_behind_Run()
    {
        var businessName = AuthenticationRequests.UniqueBusinessName();
        using var client = fixture.ApiFactory.CreateClient();
        await client.SignUpAsync(AuthenticationRequests.SignUpCommand(businessName: businessName));
        using var factoryWithoutSlugCheck = fixture.ApiFactory.WithWebHostBuilder(builder => builder.ConfigureTestServices(services =>
            services.Replace(ServiceDescriptor.Scoped<IBusinessRepository>(serviceProvider =>
                new SlugCheckSkippingBusinessRepository(ActivatorUtilities.CreateInstance<BusinessRepository>(serviceProvider))))));
        using var clientWithoutSlugCheck = factoryWithoutSlugCheck.CreateClient();
        var email = AuthenticationRequests.UniqueEmail();

        using var response = await clientWithoutSlugCheck.PostSignUpAsync(AuthenticationRequests.SignUpCommand(email, businessName));

        response.StatusCode.ShouldBe(HttpStatusCode.InternalServerError);
        await using var securityContext = fixture.CreateSecurityDbContext();
        (await securityContext.Users.AnyAsync(user => user.Email == email)).ShouldBeFalse();
    }

    private sealed class SlugCheckSkippingBusinessRepository(IBusinessRepository businessRepository) : IBusinessRepository
    {
        public void Add(Business business) => businessRepository.Add(business);

        public Task<Business?> GetCurrentAsync(CancellationToken cancellationToken) => businessRepository.GetCurrentAsync(cancellationToken);

        public Task<Business?> GetCurrentForUpdateAsync(CancellationToken cancellationToken) =>
            businessRepository.GetCurrentForUpdateAsync(cancellationToken);

        public Task<bool> IsSlugTakenAsync(string slug, CancellationToken cancellationToken) => Task.FromResult(false);
    }
}
