using System.Net.Http.Headers;
using ClassManager.Core.Domain.Businesses;
using ClassManager.Infrastructure.Persistence;
using ClassManager.Security.Persistence;
using ClassManager.Security.Tokens;
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
        BusinessRole role = BusinessRole.Owner)
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
        var testOwner = new TokenSubject(Guid.CreateVersion7(), TestOwnerEmail, businessId, role.ToString());

        return accessTokenFactory.Create(testOwner).Token;
    }

    public HttpClient CreateClientWithToken(string accessToken)
    {
        var client = ApiFactory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(BearerScheme, accessToken);
        return client;
    }

    public HttpClient CreateClientFor(Guid businessId) => CreateClientWithToken(CreateAccessToken(businessId));

    public async Task<SeededBusiness> SeedBusinessAsync()
    {
        var business = Business.Create(
            BusinessName,
            $"business-{Guid.NewGuid():N}",
            BuenosAiresTimeZoneId,
            CurrencyCode,
            DefaultCountryCallingCode,
            BusinessApiFactory.Now).Value!;

        await using var context = CreateDbContext(business.Id);
        context.Businesses.Add(business);
        await context.SaveChangesAsync();

        return new SeededBusiness(business, CreateClientFor(business.Id));
    }
}
