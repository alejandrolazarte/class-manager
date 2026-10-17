using ClassManager.Infrastructure.Persistence;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ClassManager.Api.I.Tests.Persistence.When_who_did_what_is_migrated_to_users;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_each_account_becomes_its_user_in_the_branch(ApiFixture fixture)
{
    private const string MigrationBeforeWhoDidWhat = "20261017104943_AddBrandOwnerUsers";

    [Fact]
    public async Task Then_each_account_becomes_its_user_in_the_branch_Run()
    {
        var connectionString = new SqlConnectionStringBuilder(fixture.ConnectionString)
        {
            InitialCatalog = $"WhoDidWhatMigration{Guid.NewGuid():N}",
        }.ConnectionString;
        await using var context = new AppDbContext(
            new DbContextOptionsBuilder<AppDbContext>().UseSqlServer(connectionString).Options,
            new FixedTenantContext(Guid.Empty));
        var migrator = context.GetService<IMigrator>();
        await migrator.MigrateAsync(MigrationBeforeWhoDidWhat);

        var rows = new MigrationRows();
        await context.Database.ExecuteSqlAsync(
            $"""
            INSERT INTO Organizations (Id, CreatedAt, Name) VALUES ({rows.OrganizationId}, SYSUTCDATETIME(), N'Estudio');
            INSERT INTO Businesses (Id, OrganizationId, Name, Slug, CountryCode, TimeZoneId, CurrencyCode, DefaultCountryCallingCode, NoticedAbsencesKeepStreak, CreatedAt)
            VALUES ({rows.BusinessId}, {rows.OrganizationId}, N'Estudio Centro', {rows.BusinessId.ToString("N")}, N'AR', N'America/Argentina/Buenos_Aires', N'ARS', N'54', 1, SYSUTCDATETIME()),
                   ({rows.OtherBusinessId}, {rows.OrganizationId}, N'Estudio Norte', {rows.OtherBusinessId.ToString("N")}, N'AR', N'America/Argentina/Buenos_Aires', N'ARS', N'54', 1, SYSUTCDATETIME());
            INSERT INTO Users (Id, TenantId, FullName, AccountId, CreatedAt, IsDeleted)
            VALUES ({rows.MartaUserId}, {rows.BusinessId}, N'Marta Ruiz', {rows.MartaAccountId}, SYSUTCDATETIME(), 0),
                   ({rows.MartaOtherBranchUserId}, {rows.OtherBusinessId}, N'Marta Ruiz', {rows.MartaAccountId}, SYSUTCDATETIME(), 0),
                   ({rows.LauraOtherBranchUserId}, {rows.OtherBusinessId}, N'Laura Gómez', {rows.LauraAccountId}, SYSUTCDATETIME(), 0);
            INSERT INTO Clients (Id, TenantId, FullName, PhoneNumber, CreatedAt, RegisteredByUserId)
            VALUES ({rows.ClientByMartaId}, {rows.BusinessId}, N'Ana Pérez', N'+5491122334455', SYSUTCDATETIME(), {rows.MartaAccountId}),
                   ({rows.ClientByLauraId}, {rows.BusinessId}, N'Sofía Díaz', N'+5491122334466', SYSUTCDATETIME(), {rows.LauraAccountId});
            INSERT INTO Payments (Id, TenantId, ClientId, Amount, CurrencyCode, Month, PaidOn, Method, CreatedAt, RecordedByUserId, IsDeleted)
            VALUES ({rows.PaymentByMartaId}, {rows.BusinessId}, {rows.ClientByMartaId}, 12000, N'ARS', '2026-10-01', '2026-10-05', N'Cash', SYSUTCDATETIME(), {rows.MartaAccountId}, 0),
                   ({rows.PaymentByLauraId}, {rows.BusinessId}, {rows.ClientByLauraId}, 12000, N'ARS', '2026-10-01', '2026-10-05', N'Cash', SYSUTCDATETIME(), {rows.LauraAccountId}, 0);
            INSERT INTO MemberInvitations (Id, TenantId, Email, Role, TokenHash, InvitedByUserId, CreatedAt, ExpiresAt)
            VALUES ({rows.InvitationByMartaId}, {rows.BusinessId}, N'pablo@example.com', N'Viewer', REPLICATE(N'a', 64), {rows.MartaAccountId}, SYSUTCDATETIME(), SYSUTCDATETIME()),
                   ({rows.InvitationByLauraId}, {rows.BusinessId}, N'julia@example.com', N'Viewer', REPLICATE(N'b', 64), {rows.LauraAccountId}, SYSUTCDATETIME(), SYSUTCDATETIME());
            """);

        await migrator.MigrateAsync();

        await using var tenantDbContext = new AppDbContext(
            new DbContextOptionsBuilder<AppDbContext>().UseSqlServer(connectionString).Options,
            new FixedTenantContext(rows.BusinessId));
        var registeredBy = await tenantDbContext.Clients.AsNoTracking().ToDictionaryAsync(client => client.Id, client => client.RegisteredByUserId);
        var recordedBy = await tenantDbContext.Payments.AsNoTracking().ToDictionaryAsync(payment => payment.Id, payment => payment.RecordedByUserId);
        var invitedBy = await tenantDbContext.MemberInvitations.AsNoTracking().ToDictionaryAsync(invitation => invitation.Id, invitation => invitation.InvitedByUserId);
        await tenantDbContext.Database.EnsureDeletedAsync();
        var whoDidWhat = new[]
        {
            ("Client by Marta", registeredBy[rows.ClientByMartaId]),
            ("Client by Laura", registeredBy[rows.ClientByLauraId]),
            ("Payment by Marta", recordedBy[rows.PaymentByMartaId]),
            ("Payment by Laura", recordedBy[rows.PaymentByLauraId]),
            ("Invitation by Marta", invitedBy[rows.InvitationByMartaId]),
            ("Invitation by Laura", invitedBy[rows.InvitationByLauraId]),
        };

        whoDidWhat.ShouldBe(
        [
            ("Client by Marta", (Guid?)rows.MartaUserId),
            ("Client by Laura", null),
            ("Payment by Marta", rows.MartaUserId),
            ("Payment by Laura", null),
            ("Invitation by Marta", rows.MartaUserId),
            ("Invitation by Laura", null),
        ]);
    }

    private sealed class MigrationRows
    {
        public Guid OrganizationId { get; } = Guid.NewGuid();
        public Guid BusinessId { get; } = Guid.NewGuid();
        public Guid OtherBusinessId { get; } = Guid.NewGuid();
        public Guid MartaAccountId { get; } = Guid.NewGuid();
        public Guid LauraAccountId { get; } = Guid.NewGuid();
        public Guid MartaUserId { get; } = Guid.NewGuid();
        public Guid MartaOtherBranchUserId { get; } = Guid.NewGuid();
        public Guid LauraOtherBranchUserId { get; } = Guid.NewGuid();
        public Guid ClientByMartaId { get; } = Guid.NewGuid();
        public Guid ClientByLauraId { get; } = Guid.NewGuid();
        public Guid PaymentByMartaId { get; } = Guid.NewGuid();
        public Guid PaymentByLauraId { get; } = Guid.NewGuid();
        public Guid InvitationByMartaId { get; } = Guid.NewGuid();
        public Guid InvitationByLauraId { get; } = Guid.NewGuid();
    }
}
