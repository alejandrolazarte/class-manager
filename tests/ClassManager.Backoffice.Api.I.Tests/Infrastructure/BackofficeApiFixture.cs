using ClassManager.Backoffice.Core.Operators;
using ClassManager.Backoffice.Infrastructure.Persistence;
using ClassManager.Core.Domain.Businesses;
using ClassManager.Core.Domain.Clients;
using ClassManager.Core.Domain.Organizations;
using ClassManager.Core.Domain.Students;
using ClassManager.Core.Domain.Users;
using ClassManager.Infrastructure.Persistence;
using ClassManager.Security.Accounts;
using ClassManager.Security.Persistence;
using ClassManager.Subscriptions.Billing;
using ClassManager.Subscriptions.Subscribers;
using ClassManager.Tenancy.AspNetCore.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.Azurite;
using Testcontainers.MsSql;

namespace ClassManager.Backoffice.Api.I.Tests.Infrastructure;

public sealed class BackofficeApiFixture : IAsyncLifetime, IDisposable
{
    public const string GoogleProvider = "google";

    private const string SqlServerImage = "mcr.microsoft.com/mssql/server:2022-latest";
    private const string AzuriteImage = "mcr.microsoft.com/azure-storage/azurite:3.37.0";
    private const string SkipApiVersionCheckFlag = "--skipApiVersionCheck";
    private const string PlatformOrganizationName = "Laztty";
    private const string OtherOrganizationName = "Otra marca";
    public const string ArgentinaCountryCode = "AR";
    public const string SpainCountryCode = "ES";
    private const string TimeZoneId = "America/Argentina/Buenos_Aires";
    private const string CurrencyCode = "ARS";
    private const string SpainTimeZoneId = "Europe/Madrid";
    private const string SpainCurrencyCode = "EUR";
    private const string OperatorFullName = "Operadora";
    private const string OwnerFullName = "Dueña de la marca";
    private const string BranchName = "Sede";
    private const string ClientFullName = "Familia";
    private const string StudentFullName = "Alumno";
    private const string ClientPhoneNumber = "1155550000";

    private readonly MsSqlContainer _container = new MsSqlBuilder(SqlServerImage).Build();
    private readonly AzuriteContainer _storageContainer = new AzuriteBuilder(AzuriteImage)
        .WithInMemoryPersistence()
        .WithCommand(SkipApiVersionCheckFlag)
        .Build();
    private readonly Organization _platformOrganization = Organization.Create(PlatformOrganizationName, BackofficeApiFactory.Now).Value!;
    private BackofficeApiFactory? _apiFactory;

    public BackofficeApiFactory ApiFactory => _apiFactory ?? throw new InvalidOperationException(nameof(InitializeAsync));

    public async Task InitializeAsync()
    {
        await Task.WhenAll(_container.StartAsync(), _storageContainer.StartAsync());
        await using (var context = CreateAppDbContext(0L))
        {
            await context.Database.MigrateAsync();
            context.Organizations.Add(_platformOrganization);
            await context.SaveChangesAsync();
        }

        await using (var securityContext = new SecurityDbContext(new DbContextOptionsBuilder<SecurityDbContext>()
            .UseSqlServer(_container.GetConnectionString(), SecurityDbContext.ConfigureSqlServer)
            .UseIdentitySchema()
            .Options))
        {
            await securityContext.Database.MigrateAsync();
        }

        await using (var backofficeContext = CreateBackofficeDbContext())
        {
            await backofficeContext.Database.MigrateAsync();
        }

        _apiFactory = new BackofficeApiFactory(_container.GetConnectionString(), _storageContainer.GetConnectionString(), _platformOrganization.Id);
    }

    public string StorageConnectionString => _storageContainer.GetConnectionString();

    public async Task DisposeAsync()
    {
        await _container.DisposeAsync();
        await _storageContainer.DisposeAsync();
    }

    public void Dispose() => _apiFactory?.Dispose();

    public BackofficeDbContext CreateBackofficeDbContext() =>
        new(new DbContextOptionsBuilder<BackofficeDbContext>()
            .UseSqlServer(_container.GetConnectionString(), BackofficeDbContext.ConfigureSqlServer)
            .Options);

    public Task<SeededOperator> SeedOperatorAsync(BusinessRole role = BusinessRole.PlatformOperator, bool isEnabled = true) =>
        SeedOperatorAsync(_platformOrganization.Id, role, isEnabled);

    public Task<SeededOperator> SeedPlatformTeamMemberAsync() =>
        SeedOperatorAsync(_platformOrganization.Id, BusinessRole.Viewer, isEnabled: false);

    public async Task<SeededOperator> SeedOperatorOfAnotherOrganizationAsync()
    {
        var organization = Organization.Create(OtherOrganizationName, BackofficeApiFactory.Now).Value!;
        await using (var context = CreateAppDbContext(0L))
        {
            context.Organizations.Add(organization);
            await context.SaveChangesAsync();
        }

        return await SeedOperatorAsync(organization.Id, BusinessRole.PlatformOperator, isEnabled: true);
    }

    public async Task<SeededBrand> SeedBrandAsync(
        string name,
        IReadOnlyList<int> studentsPerBranch,
        Func<long, IReadOnlyList<Subscription>> subscriptionsOf,
        string countryCode = ArgentinaCountryCode)
    {
        var isSpain = countryCode == SpainCountryCode;
        var organization = Organization.Create(name, BackofficeApiFactory.Now).Value!;
        var ownerEmail = $"owner-{TestIds.Next()}@example.com";
        var branchIds = new List<long>();
        await using (var context = CreateAppDbContext(0L))
        {
            context.Organizations.Add(organization);
            await context.SaveChangesAsync();
            context.Subscriptions.AddRange(subscriptionsOf(organization.Id));
            await context.SaveChangesAsync();
        }

        foreach (var (studentCount, branchIndex) in studentsPerBranch.Select((count, index) => (count, index)))
        {
            await using var lookupContext = CreateAppDbContext(0L);
            var country = await lookupContext.Countries.AsNoTracking().SingleAsync(candidate => candidate.Code == countryCode);
            var business = Business.Create(
                organization.Id,
                $"{BranchName} {branchIndex + 1}",
                $"branch-{TestIds.Next()}",
                country,
                isSpain ? SpainTimeZoneId : TimeZoneId,
                isSpain ? SpainCurrencyCode : CurrencyCode,
                BackofficeApiFactory.Now).Value!;
            lookupContext.Countries.Attach(country);
            lookupContext.Businesses.Add(business);
            await lookupContext.SaveChangesAsync();
            await using var context = CreateAppDbContext(business.Id);
            branchIds.Add(business.Id);
            if (branchIndex == 0)
            {
                var ownerAccountId = Guid.CreateVersion7();
                context.Users.Add(User.CreateForAccount(ownerAccountId, OwnerFullName, ownerEmail, BackofficeApiFactory.Now));
                context.OrganizationMembers.Add(OrganizationMember.CreateBrandOwner(organization.Id, ownerAccountId));
            }

            var phoneNumber = PhoneNumber.Create(ClientPhoneNumber, business.DefaultCountryCallingCode).Value!;
            for (var studentIndex = 0; studentIndex < studentCount; studentIndex++)
            {
                var client = Client.Create(ClientFullName, phoneNumber, null, null, BackofficeApiFactory.Now).Value!;
                context.Clients.Add(client);
                context.Students.Add(Student.Create(client, StudentFullName, null, null, DateOnly.FromDateTime(BackofficeApiFactory.Now.UtcDateTime), BackofficeApiFactory.Now).Value!);
            }

            await context.SaveChangesAsync();
        }

        return new SeededBrand(organization.Id, ownerEmail, branchIds);
    }

    public async Task SeedBillingCustomerAsync(long brandId, string externalCustomerId)
    {
        await using var context = CreateAppDbContext(0L);
        context.BillingCustomers.Add(BillingCustomer.Create(brandId, externalCustomerId));
        await context.SaveChangesAsync();
    }

    private async Task<SeededOperator> SeedOperatorAsync(long organizationId, BusinessRole role, bool isEnabled)
    {
        var accountId = Guid.CreateVersion7();
        var email = $"operator-{TestIds.Next()}@example.com";
        var subject = $"google-{TestIds.Next()}";
        long memberId;

        using (var scope = ApiFactory.Services.CreateScope())
        {
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var created = await userManager.CreateAsync(new ApplicationUser { Id = accountId, UserName = email, Email = email, FullName = OperatorFullName });
            created.Succeeded.ShouldBeTrue();
        }

        await using (var lookupContext = CreateAppDbContext(0L))
        {
            var argentina = await lookupContext.Countries.AsNoTracking().SingleAsync(country => country.Code == ArgentinaCountryCode);
            var business = Business.Create(organizationId, PlatformOrganizationName, $"business-{TestIds.Next()}", argentina, TimeZoneId, CurrencyCode, BackofficeApiFactory.Now).Value!;
            lookupContext.Countries.Attach(argentina);
            lookupContext.Businesses.Add(business);
            await lookupContext.SaveChangesAsync();
            await using var context = CreateAppDbContext(business.Id);
            var member = BusinessMember.Create(
                business.Id,
                User.CreateForAccount(accountId, OperatorFullName, null, BackofficeApiFactory.Now),
                MemberRole.System(role),
                instructorId: null).Value!;
            context.BusinessMembers.Add(member);
            await context.SaveChangesAsync();
            memberId = member.Id;
        }

        if (isEnabled)
        {
            await using var backofficeContext = CreateBackofficeDbContext();
            backofficeContext.BackofficeLogins.Add(BackofficeLogin.Enable(
                new ExternalIdentity(GoogleProvider, subject, email), accountId, BackofficeApiFactory.Now, enabledByUserId: null));
            await backofficeContext.SaveChangesAsync();
        }

        return new SeededOperator(accountId, memberId, subject, email);
    }

    private AppDbContext CreateAppDbContext(long businessId)
    {
        var tenantContext = new FixedTenantContext(businessId);
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlServer(_container.GetConnectionString())
            .AddInterceptors(new TenantStampingSaveChangesInterceptor(tenantContext))
            .Options;
        return new AppDbContext(options, tenantContext);
    }
}

public sealed record SeededOperator(Guid AccountId, long MemberId, string Subject, string Email);

public sealed record SeededBrand(long Id, string OwnerEmail, IReadOnlyList<long> BranchIds);
