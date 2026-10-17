using ClassManager.Infrastructure.Persistence;
using ClassManager.Security.Persistence;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ClassManager.Api.I.Tests.Persistence.When_people_are_migrated_to_users;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_each_person_has_one_user(ApiFixture fixture)
{
    private const string MigrationBeforeUsers = "20261017025643_UseDeletedAtForSoftDelete";

    [Fact]
    public async Task Then_each_person_has_one_user_Run()
    {
        var connectionString = new SqlConnectionStringBuilder(fixture.ConnectionString)
        {
            InitialCatalog = $"UsersMigration{Guid.NewGuid():N}",
        }.ConnectionString;
        await using var context = new AppDbContext(
            new DbContextOptionsBuilder<AppDbContext>().UseSqlServer(connectionString).Options,
            new FixedTenantContext(Guid.Empty));
        var migrator = context.GetService<IMigrator>();
        await migrator.MigrateAsync(MigrationBeforeUsers);
        await using (var securityContext = new SecurityDbContext(
            new DbContextOptionsBuilder<SecurityDbContext>().UseSqlServer(connectionString, SecurityDbContext.ConfigureSqlServer).Options))
        {
            await securityContext.Database.MigrateAsync();
        }

        var people = new MigrationPeople();
        await context.Database.ExecuteSqlAsync(
            $"""
            INSERT INTO [identity].[AspNetUsers] (Id, FullName, Email, EmailConfirmed, PhoneNumberConfirmed, TwoFactorEnabled, LockoutEnabled, AccessFailedCount)
            VALUES ({people.OwnerAccountId}, N'Marta Ruiz', N'marta@example.com', 1, 0, 0, 0, 0),
                   ({people.InstructorAccountId}, N'Laura Gómez', N'laura@example.com', 1, 0, 0, 0, 0),
                   ({people.ChildAccountId}, N'Tomás Pérez', N'tomas@example.com', 1, 0, 0, 0, 0);
            INSERT INTO Organizations (Id, CreatedAt, Name) VALUES ({people.OrganizationId}, SYSUTCDATETIME(), N'Estudio');
            INSERT INTO Businesses (Id, OrganizationId, Name, Slug, CountryCode, TimeZoneId, CurrencyCode, DefaultCountryCallingCode, NoticedAbsencesKeepStreak, CreatedAt)
            VALUES ({people.BusinessId}, {people.OrganizationId}, N'Estudio', {people.BusinessId.ToString("N")}, N'AR', N'America/Argentina/Buenos_Aires', N'ARS', N'54', 1, SYSUTCDATETIME());
            INSERT INTO Clients (Id, TenantId, FullName, PhoneNumber, Email, CreatedAt)
            VALUES ({people.ClientId}, {people.BusinessId}, N'Ana Pérez', N'+5491122334455', N'ana@example.com', SYSUTCDATETIME());
            INSERT INTO Students (Id, TenantId, ClientId, FullName, BirthDate, CreatedAt)
            VALUES ({people.AttendingContactId}, {people.BusinessId}, {people.ClientId}, N'Ana Pérez', '1990-05-10', SYSUTCDATETIME()),
                   ({people.ChildId}, {people.BusinessId}, {people.ClientId}, N'Tomás Pérez', '2014-03-14', SYSUTCDATETIME());
            INSERT INTO Instructors (Id, TenantId, FullName, Email, IsActive)
            VALUES ({people.InstructorId}, {people.BusinessId}, N'Laura Gómez', N'laura@example.com', 1);
            INSERT INTO BusinessMembers (Id, TenantId, UserId, Role, InstructorId, IsDeleted)
            VALUES ({people.OwnerMemberId}, {people.BusinessId}, {people.OwnerAccountId}, N'BranchOwner', NULL, 0),
                   ({people.InstructorMemberId}, {people.BusinessId}, {people.InstructorAccountId}, N'Instructor', {people.InstructorId}, 0);
            INSERT INTO ClientAccounts (Id, TenantId, ClientId, StudentId, UserId, CreatedAt)
            VALUES ({people.ChildClientAccountId}, {people.BusinessId}, {people.ClientId}, {people.ChildId}, {people.ChildAccountId}, SYSUTCDATETIME());
            """);

        await migrator.MigrateAsync();

        var tenantContext = new FixedTenantContext(people.BusinessId);
        await using var tenantDbContext = new AppDbContext(
            new DbContextOptionsBuilder<AppDbContext>().UseSqlServer(connectionString).Options,
            tenantContext);
        var users = await tenantDbContext.Users.AsNoTracking().ToListAsync();
        var client = await tenantDbContext.Clients.AsNoTracking().SingleAsync();
        var students = await tenantDbContext.Students.AsNoTracking().ToDictionaryAsync(student => student.Id);
        var instructor = await tenantDbContext.Instructors.AsNoTracking().SingleAsync();
        var members = await tenantDbContext.BusinessMembers.AsNoTracking().ToDictionaryAsync(member => member.Id);
        var childClientAccount = await tenantDbContext.ClientAccounts.AsNoTracking().SingleAsync();
        await tenantDbContext.Database.EnsureDeletedAsync();
        var usersById = users.ToDictionary(user => user.Id);
        var links = new[]
        {
            ("Ana as client", usersById[client.UserId!.Value].FullName, usersById[client.UserId.Value].AccountId),
            ("Ana as student", usersById[students[people.AttendingContactId].UserId!.Value].FullName, (Guid?)null),
            ("Tomás", usersById[students[people.ChildId].UserId!.Value].FullName, usersById[students[people.ChildId].UserId!.Value].AccountId),
            ("Tomás account", usersById[childClientAccount.UserId!.Value].FullName, childClientAccount.AccountId),
            ("Laura as instructor", usersById[instructor.UserId!.Value].FullName, usersById[instructor.UserId.Value].AccountId),
            ("Laura as member", usersById[members[people.InstructorMemberId].UserId!.Value].FullName, members[people.InstructorMemberId].AccountId),
            ("Marta as member", usersById[members[people.OwnerMemberId].UserId!.Value].FullName, usersById[members[people.OwnerMemberId].UserId!.Value].AccountId),
        };

        links.ShouldBe(
        [
            ("Ana as client", "Ana Pérez", (Guid?)null),
            ("Ana as student", "Ana Pérez", null),
            ("Tomás", "Tomás Pérez", people.ChildAccountId),
            ("Tomás account", "Tomás Pérez", people.ChildAccountId),
            ("Laura as instructor", "Laura Gómez", people.InstructorAccountId),
            ("Laura as member", "Laura Gómez", people.InstructorAccountId),
            ("Marta as member", "Marta Ruiz", people.OwnerAccountId),
        ]);
        users.Count.ShouldBe(4);
        students[people.AttendingContactId].UserId.ShouldBe(client.UserId);
        usersById[client.UserId.Value].BirthDate.ShouldBe(new DateOnly(1990, 5, 10));
    }

    private sealed class MigrationPeople
    {
        public Guid OrganizationId { get; } = Guid.NewGuid();
        public Guid BusinessId { get; } = Guid.NewGuid();
        public Guid ClientId { get; } = Guid.NewGuid();
        public Guid AttendingContactId { get; } = Guid.NewGuid();
        public Guid ChildId { get; } = Guid.NewGuid();
        public Guid InstructorId { get; } = Guid.NewGuid();
        public Guid OwnerMemberId { get; } = Guid.NewGuid();
        public Guid InstructorMemberId { get; } = Guid.NewGuid();
        public Guid ChildClientAccountId { get; } = Guid.NewGuid();
        public Guid OwnerAccountId { get; } = Guid.NewGuid();
        public Guid InstructorAccountId { get; } = Guid.NewGuid();
        public Guid ChildAccountId { get; } = Guid.NewGuid();
    }
}
