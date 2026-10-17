using System.Net.Http.Headers;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Domain.Businesses;
using ClassManager.Core.Domain.Countries;
using ClassManager.Core.Domain.Organizations;
using ClassManager.Core.Domain.Students;
using ClassManager.Core.Domain.Subscriptions;
using ClassManager.Core.Domain.Users;
using ClassManager.Core.UseCases.Students;
using ClassManager.Infrastructure.Persistence;
using ClassManager.Security.Persistence;
using ClassManager.Security.Tokens;
using ClassManager.Storage.AzureBlob.Blobs;
using ClassManager.Subscriptions.Subscribers;
using ClassManager.Tenancy.AspNetCore.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Time.Testing;
using Testcontainers.Azurite;
using Testcontainers.MsSql;

namespace ClassManager.Api.I.Tests.Infrastructure;

public sealed class ApiFixture : IAsyncLifetime, IDisposable
{
    public const string BuenosAiresTimeZoneId = "America/Argentina/Buenos_Aires";
    public const string CurrencyCode = "ARS";
    public const string DefaultCountryCallingCode = "54";
    public const string ArgentinaCountryCode = "AR";
    public const string SpainCountryCode = "ES";
    public const string BearerScheme = "Bearer";

    private const string SeededStudentsClientName = "Cliente sembrado";
    private const string SeededStudentsPhoneNumber = "11 4000-1000";
    private const string SqlServerImage = "mcr.microsoft.com/mssql/server:2022-latest";
    private const string AzuriteImage = "mcr.microsoft.com/azure-storage/azurite:3.37.0";
    private const string SkipApiVersionCheckFlag = "--skipApiVersionCheck";
    private const string ConnectionStringsSection = "ConnectionStrings";
    private const string BusinessName = "Test business";
    private const string TestOwnerEmail = "owner@test.local";
    private const string SeededOwnerFullName = "Dueña sembrada";
    private const string SeededMemberFullName = "Miembro sembrado";

    private readonly MsSqlContainer _container = new MsSqlBuilder(SqlServerImage).Build();
    private readonly AzuriteContainer _storageContainer = new AzuriteBuilder(AzuriteImage)
        .WithInMemoryPersistence()
        .WithCommand(SkipApiVersionCheckFlag)
        .Build();
    private BusinessApiFactory? _apiFactory;

    public string ConnectionString => _container.GetConnectionString();

    public HttpClient AnonymousClient { get; } = new();

    public BusinessApiFactory ApiFactory => _apiFactory ?? throw new InvalidOperationException(nameof(InitializeAsync));

    public async Task InitializeAsync()
    {
        await Task.WhenAll(_container.StartAsync(), _storageContainer.StartAsync());
        await using (var context = CreateDbContext(Guid.Empty))
        {
            await context.Database.MigrateAsync();
        }

        await using (var securityContext = CreateSecurityDbContext())
        {
            await securityContext.Database.MigrateAsync();
        }

        var storageSettings = new Dictionary<string, string>
        {
            [$"{ConnectionStringsSection}:{BlobStorageOptions.ConnectionStringName}"] = _storageContainer.GetConnectionString(),
        };
        _apiFactory = new BusinessApiFactory(ConnectionString, new FakeTimeProvider(BusinessApiFactory.Now), storageSettings);
    }

    public async Task DisposeAsync()
    {
        await _container.DisposeAsync();
        await _storageContainer.DisposeAsync();
    }

    public void Dispose()
    {
        _apiFactory?.Dispose();
        AnonymousClient.Dispose();
    }

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
        Guid? accountId = null,
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
        var subjectAccountId = accountId ?? Guid.CreateVersion7();
        var testOwner = new TokenSubject(subjectAccountId, TestOwnerEmail, businessId, nameof(BusinessRole.BranchOwner), kind, SessionUserClaimsOf(businessId, subjectAccountId));

        return accessTokenFactory.Create(testOwner).Token;
    }

    public HttpClient CreateClientWithToken(string accessToken)
    {
        var client = ApiFactory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(BearerScheme, accessToken);
        return client;
    }

    public HttpClient CreateClientFor(Guid businessId, Guid accountId) => CreateClientWithToken(CreateAccessToken(businessId, accountId: accountId));

    public Task<HttpClient> SeedMemberAsync(Guid businessId, BusinessRole role, Guid? instructorId = null) =>
        SeedMemberAsync(businessId, MemberRole.System(role), instructorId);

    public async Task<HttpClient> SeedMemberAsync(Guid businessId, MemberRole role, Guid? instructorId = null)
    {
        var accountId = Guid.CreateVersion7();
        await using var context = CreateDbContext(businessId);
        var instructorUser = instructorId is null
            ? null
            : await context.Instructors.Where(instructor => instructor.Id == instructorId).Select(instructor => instructor.User).SingleOrDefaultAsync();
        if (instructorUser is { AccountId: null })
        {
            instructorUser.LinkAccount(accountId);
        }
        else
        {
            context.Users.Add(UserOfAccount(businessId, SeededMemberFullName, accountId));
        }

        context.BusinessMembers.Add(BusinessMember.Create(businessId, accountId, role, instructorId).Value!);
        await context.SaveChangesAsync();

        return CreateClientFor(businessId, accountId);
    }

    public async Task ChangePlanAsync(Guid businessId, string planCode)
    {
        await using var context = CreateDbContext(businessId);
        var organizationId = await context.Businesses.Where(business => business.Id == businessId).Select(business => business.OrganizationId).SingleAsync();
        var current = await context.Subscriptions.SingleAsync(subscription => subscription.SubscriberId == organizationId && !subscription.IsDeleted);
        current.Delete(BusinessApiFactory.Now);
        context.Subscriptions.Add(Subscription.Start(organizationId, planCode, price: 0m, CurrencyCode, note: null, BusinessApiFactory.Now));
        await context.SaveChangesAsync();
    }

    public async Task EndSubscriptionAsync(Guid businessId)
    {
        const int DaysSinceItStarted = 40;
        var today = DateOnly.FromDateTime(BusinessApiFactory.Now.UtcDateTime);
        await using var context = CreateDbContext(businessId);
        var organizationId = await context.Businesses.Where(business => business.Id == businessId).Select(business => business.OrganizationId).SingleAsync();
        var current = await context.Subscriptions.SingleAsync(subscription => subscription.SubscriberId == organizationId && !subscription.IsDeleted);
        current.Delete(BusinessApiFactory.Now);
        var expiredSubscription = Subscription.Start(
            organizationId,
            PlanCodes.Free,
            price: 0m,
            CurrencyCode,
            note: null,
            BusinessApiFactory.Now.AddDays(-DaysSinceItStarted));
        expiredSubscription.Expire(today.AddDays(-1));
        context.Subscriptions.Add(expiredSubscription);
        await context.SaveChangesAsync();
    }

    public async Task SeedStudentsAsync(SeededBusiness business, int studentCount)
    {
        var client = await business.HttpClient.RegisterClientAsync(
            fullName: SeededStudentsClientName,
            phoneNumber: SeededStudentsPhoneNumber,
            students: [new NewStudent("Alumno 1", null, null)]);
        var today = DateOnly.FromDateTime(BusinessApiFactory.Now.UtcDateTime);
        await using var context = CreateDbContext(business.Business.Id);
        for (var studentNumber = 2; studentNumber <= studentCount; studentNumber++)
        {
            context.Students.Add(Student.Create(client.Id, $"Alumno {studentNumber}", null, null, today, BusinessApiFactory.Now).Value!);
        }

        await context.SaveChangesAsync();
    }

    public async Task AddFeatureAsync(SeededBusiness business, string featureCode, int? limit = null)
    {
        await using var context = CreateDbContext(business.Business.Id);
        var subscriptionId = await context.Subscriptions
            .Where(subscription => subscription.SubscriberId == business.Business.OrganizationId && !subscription.IsDeleted)
            .Select(subscription => subscription.Id)
            .SingleAsync();
        context.SubscriptionFeatures.Add(SubscriptionFeature.Create(
            subscriptionId,
            featureCode,
            price: 0m,
            CurrencyCode,
            limit,
            expiredOn: null,
            BusinessApiFactory.Now));
        await context.SaveChangesAsync();
    }

    public async Task<Country> FindCountryAsync(string countryCode)
    {
        await using var context = CreateDbContext(Guid.Empty);
        return await context.Countries.AsNoTracking().SingleAsync(country => country.Code == countryCode);
    }

    public async Task<SeededBusiness> SeedBusinessAsync(string planCode = PlanCodes.Enterprise)
    {
        var organization = Organization.Create(BusinessName, BusinessApiFactory.Now).Value!;
        var ownerAccountId = Guid.CreateVersion7();
        var argentina = await FindCountryAsync(ArgentinaCountryCode);
        var business = Business.Create(
            organization.Id,
            BusinessName,
            $"business-{Guid.NewGuid():N}",
            argentina,
            BuenosAiresTimeZoneId,
            CurrencyCode,
            BusinessApiFactory.Now).Value!;

        await using var context = CreateDbContext(business.Id);
        context.Countries.Attach(argentina);
        context.Organizations.Add(organization);
        context.OrganizationMembers.Add(OrganizationMember.CreateBrandOwner(organization.Id, ownerAccountId));
        context.Businesses.Add(business);
        context.Users.Add(UserOfAccount(business.Id, SeededOwnerFullName, ownerAccountId));
        context.BusinessMembers.Add(BusinessMember.CreateBranchOwner(business.Id, ownerAccountId));
        context.Subscriptions.Add(Subscription.Start(
            organization.Id,
            planCode,
            price: 0m,
            CurrencyCode,
            note: null,
            BusinessApiFactory.Now));
        await context.SaveChangesAsync();

        return new SeededBusiness(business, CreateClientFor(business.Id, ownerAccountId), ownerAccountId);
    }

    private static User UserOfAccount(Guid businessId, string fullName, Guid accountId)
    {
        var user = User.Create(businessId, fullName, BusinessApiFactory.Now);
        user.LinkAccount(accountId);
        return user;
    }

    private Dictionary<string, string>? SessionUserClaimsOf(Guid businessId, Guid accountId)
    {
        using var context = CreateDbContext(businessId);
        var userId = context.Users
            .Where(user => user.AccountId == accountId)
            .Select(user => (Guid?)user.Id)
            .FirstOrDefault();
        return userId is { } linkedUserId ? new Dictionary<string, string> { [SessionClaimTypes.UserId] = linkedUserId.ToString() } : null;
    }
}
