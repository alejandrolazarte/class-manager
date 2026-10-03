using System.Net.Http.Headers;
using ClassManager.Core.Domain.Businesses;
using ClassManager.Core.Domain.Organizations;
using ClassManager.Core.Domain.Subscriptions;
using ClassManager.Infrastructure.Persistence;
using ClassManager.Security.Persistence;
using ClassManager.Security.Tokens;
using ClassManager.Subscriptions.Subscribers;
using ClassManager.Tenancy.AspNetCore.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Time.Testing;
using Testcontainers.MsSql;

namespace ClassManager.Api.I.Tests.Infrastructure;

public sealed class ApiFixture : IAsyncLifetime, IDisposable
{
    public const string BuenosAiresTimeZoneId = "America/Argentina/Buenos_Aires";
    public const string CurrencyCode = "ARS";
    public const string DefaultCountryCallingCode = "54";
    public const string BearerScheme = "Bearer";

    private const string SqlServerImage = "mcr.microsoft.com/mssql/server:2022-latest";
    private const string BusinessName = "Test business";
    private const string TestOwnerEmail = "owner@test.local";

    private readonly MsSqlContainer _container = new MsSqlBuilder(SqlServerImage).Build();
    private BusinessApiFactory? _apiFactory;

    public string ConnectionString => _container.GetConnectionString();

    public BusinessApiFactory ApiFactory => _apiFactory ?? throw new InvalidOperationException(nameof(InitializeAsync));

    public async Task InitializeAsync()
    {
        await _container.StartAsync();
        await using (var context = CreateDbContext(Guid.Empty))
        {
            await context.Database.MigrateAsync();
        }

        await using (var securityContext = CreateSecurityDbContext())
        {
            await securityContext.Database.MigrateAsync();
        }

        _apiFactory = new BusinessApiFactory(ConnectionString);
    }

    public Task DisposeAsync() => _container.DisposeAsync().AsTask();

    public void Dispose() => _apiFactory?.Dispose();

    public AppDbContext CreateDbContext(Guid businessId)
    {
        var tenantContext = new FixedTenantContext(businessId);
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlServer(ConnectionString)
            .AddInterceptors(new TenantStampingSaveChangesInterceptor(tenantContext))
            .Options;

        return new AppDbContext(options, tenantContext);
    }

    public SecurityDbContext CreateSecurityDbContext()
    {
        var options = new DbContextOptionsBuilder<SecurityDbContext>()
            .UseSqlServer(ConnectionString, SecurityDbContext.ConfigureSqlServer)
            .Options;

        return new SecurityDbContext(options);
    }

    public string CreateAccessToken(
        Guid businessId,
        TimeProvider? timeProvider = null,
        string? signingKey = null,
        Guid? userId = null,
        string? kind = null)
    {
        var configuredOptions = ApiFactory.Services.GetRequiredService<IOptions<JwtOptions>>().Value;
        var options = new JwtOptions
        {
            Issuer = configuredOptions.Issuer,
            Audience = configuredOptions.Audience,
            SigningKey = signingKey ?? configuredOptions.SigningKey,
            TenantClaimType = configuredOptions.TenantClaimType,
            AccessTokenLifetime = configuredOptions.AccessTokenLifetime,
            RefreshTokenLifetime = configuredOptions.RefreshTokenLifetime,
        };
        var accessTokenFactory = new JwtAccessTokenFactory(Options.Create(options), timeProvider ?? new FakeTimeProvider(BusinessApiFactory.Now));
        var testOwner = new TokenSubject(userId ?? Guid.CreateVersion7(), TestOwnerEmail, businessId, nameof(BusinessRole.BranchOwner), kind);

        return accessTokenFactory.Create(testOwner).Token;
    }

    public HttpClient CreateClientWithToken(string accessToken)
    {
        var client = ApiFactory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(BearerScheme, accessToken);
        return client;
    }

    public HttpClient CreateClientFor(Guid businessId, Guid userId) => CreateClientWithToken(CreateAccessToken(businessId, userId: userId));

    public Task<HttpClient> SeedMemberAsync(Guid businessId, BusinessRole role, Guid? instructorId = null) =>
        SeedMemberAsync(businessId, MemberRole.System(role), instructorId);

    public async Task<HttpClient> SeedMemberAsync(Guid businessId, MemberRole role, Guid? instructorId = null)
    {
        var userId = Guid.CreateVersion7();
        await using var context = CreateDbContext(businessId);
        context.BusinessMembers.Add(BusinessMember.Create(businessId, userId, role, instructorId).Value!);
        await context.SaveChangesAsync();

        return CreateClientFor(businessId, userId);
    }

    public async Task ChangePlanAsync(Guid businessId, string planCode)
    {
        await using var context = CreateDbContext(businessId);
        var organizationId = await context.Businesses.Where(business => business.Id == businessId).Select(business => business.OrganizationId).SingleAsync();
        context.Subscriptions.RemoveRange(context.Subscriptions.Where(subscription => subscription.SubscriberId == organizationId));
        await context.SaveChangesAsync();
        context.Subscriptions.Add(Subscription.Start(
            organizationId,
            planCode,
            price: 0m,
            CurrencyCode,
            DateOnly.FromDateTime(BusinessApiFactory.Now.UtcDateTime),
            note: null,
            BusinessApiFactory.Now));
        await context.SaveChangesAsync();
    }

    public async Task AddFeatureAsync(SeededBusiness business, string featureCode, int? limit = null)
    {
        await using var context = CreateDbContext(business.Business.Id);
        var subscriptionId = await context.Subscriptions
            .Where(subscription => subscription.SubscriberId == business.Business.OrganizationId && subscription.EndsOn == null)
            .Select(subscription => subscription.Id)
            .SingleAsync();
        context.SubscriptionFeatures.Add(SubscriptionFeature.Create(
            subscriptionId,
            featureCode,
            price: 0m,
            CurrencyCode,
            limit,
            DateOnly.FromDateTime(BusinessApiFactory.Now.UtcDateTime),
            endsOn: null));
        await context.SaveChangesAsync();
    }

    public async Task<SeededBusiness> SeedBusinessAsync(string planCode = PlanCodes.Enterprise)
    {
        var organization = Organization.Create(BusinessName, BusinessApiFactory.Now).Value!;
        var ownerUserId = Guid.CreateVersion7();
        var business = Business.Create(
            organization.Id,
            BusinessName,
            $"business-{Guid.NewGuid():N}",
            BuenosAiresTimeZoneId,
            CurrencyCode,
            DefaultCountryCallingCode,
            BusinessApiFactory.Now).Value!;

        await using var context = CreateDbContext(business.Id);
        context.Organizations.Add(organization);
        context.OrganizationMembers.Add(OrganizationMember.CreateBrandOwner(organization.Id, ownerUserId));
        context.Businesses.Add(business);
        context.BusinessMembers.Add(BusinessMember.CreateBranchOwner(business.Id, ownerUserId));
        context.Subscriptions.Add(Subscription.Start(
            organization.Id,
            planCode,
            price: 0m,
            CurrencyCode,
            DateOnly.FromDateTime(BusinessApiFactory.Now.UtcDateTime),
            note: null,
            BusinessApiFactory.Now));
        await context.SaveChangesAsync();

        return new SeededBusiness(business, CreateClientFor(business.Id, ownerUserId), ownerUserId);
    }
}
