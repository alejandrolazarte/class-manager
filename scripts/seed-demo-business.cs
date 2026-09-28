#:package Microsoft.Data.SqlClient@6.1.1
#:package Microsoft.Extensions.Identity.Core@10.0.12

using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;

const string ConnectionStringVariable = "BUSINESS_DATABASE";
const string OwnerPasswordVariable = "DEMO_OWNER_PASSWORD";
const string UsageMessage =
    "Usage: dotnet run scripts/seed-demo-business.cs -- \"<connection string>\" \"<demo owner password>\" (or set BUSINESS_DATABASE and DEMO_OWNER_PASSWORD).";
const int MinimumPasswordLength = 10;
const string PasswordTooShortMessage = "The demo owner password must have at least 10 characters.";
const string DemoOwnerEmail = "owner@demo.local";
const string DemoOwnerFullName = "Demo owner";

var demoOrganizationId = Guid.Parse("0192f0c3-0000-7000-8000-000000000001");
var demoBrandOwnerMembershipId = Guid.Parse("0192f0c4-0000-7000-8000-000000000001");
var demoBusinessId = Guid.Parse("0192f0c0-0000-7000-8000-000000000001");
var demoOwnerId = Guid.Parse("0192f0c1-0000-7000-8000-000000000001");
var demoOwnerMembershipId = Guid.Parse("0192f0c2-0000-7000-8000-000000000001");
var connectionString = args.ElementAtOrDefault(0) ?? Environment.GetEnvironmentVariable(ConnectionStringVariable);
var ownerPassword = args.ElementAtOrDefault(1) ?? Environment.GetEnvironmentVariable(OwnerPasswordVariable);
if (string.IsNullOrWhiteSpace(connectionString) || string.IsNullOrEmpty(ownerPassword))
{
    Console.Error.WriteLine(UsageMessage);
    return 1;
}

if (ownerPassword.Length < MinimumPasswordLength)
{
    Console.Error.WriteLine(PasswordTooShortMessage);
    return 1;
}

await using var connection = new SqlConnection(connectionString);
await connection.OpenAsync();
await using var transaction = (SqlTransaction)await connection.BeginTransactionAsync();

await ExecuteAsync(
    """
    IF NOT EXISTS (SELECT 1 FROM Organizations WHERE Id = @Id)
        INSERT INTO Organizations (Id, Name, CreatedAt) VALUES (@Id, N'Demo business', SYSDATETIMEOFFSET());
    """,
    ("@Id", demoOrganizationId));

await ExecuteAsync(
    """
    IF NOT EXISTS (SELECT 1 FROM Businesses WHERE Id = @Id)
        INSERT INTO Businesses (Id, OrganizationId, Name, Slug, TimeZoneId, CurrencyCode, DefaultCountryCallingCode, CreatedAt)
        VALUES (@Id, @OrganizationId, N'Demo business', N'demo-business', N'America/Argentina/Buenos_Aires', N'ARS', N'54', SYSDATETIMEOFFSET());
    """,
    ("@Id", demoBusinessId), ("@OrganizationId", demoOrganizationId));

var ownerPasswordHash = new PasswordHasher<object>().HashPassword(new object(), ownerPassword);
await ExecuteAsync(
    """
    IF NOT EXISTS (SELECT 1 FROM [identity].[AspNetUsers] WHERE Id = @Id)
        INSERT INTO [identity].[AspNetUsers]
            (Id, FullName, UserName, NormalizedUserName, Email, NormalizedEmail, EmailConfirmed, PasswordHash,
             SecurityStamp, ConcurrencyStamp, PhoneNumberConfirmed, TwoFactorEnabled, LockoutEnabled, AccessFailedCount)
        VALUES
            (@Id, @FullName, @Email, UPPER(@Email), @Email, UPPER(@Email), 0, @PasswordHash,
             @SecurityStamp, @ConcurrencyStamp, 0, 0, 1, 0);
    ELSE
        UPDATE [identity].[AspNetUsers]
        SET PasswordHash = @PasswordHash, SecurityStamp = @SecurityStamp, AccessFailedCount = 0, LockoutEnd = NULL
        WHERE Id = @Id;
    """,
    ("@Id", demoOwnerId), ("@FullName", DemoOwnerFullName), ("@Email", DemoOwnerEmail), ("@PasswordHash", ownerPasswordHash),
    ("@SecurityStamp", Guid.NewGuid().ToString("N").ToUpperInvariant()), ("@ConcurrencyStamp", Guid.NewGuid().ToString()));

await ExecuteAsync(
    """
    IF NOT EXISTS (SELECT 1 FROM BusinessMembers WHERE Id = @Id)
        INSERT INTO BusinessMembers (Id, TenantId, UserId, Role) VALUES (@Id, @TenantId, @UserId, N'BranchOwner');
    """,
    ("@Id", demoOwnerMembershipId), ("@TenantId", demoBusinessId), ("@UserId", demoOwnerId));

await ExecuteAsync(
    """
    IF NOT EXISTS (SELECT 1 FROM OrganizationMembers WHERE Id = @Id)
        INSERT INTO OrganizationMembers (Id, OrganizationId, UserId, Role) VALUES (@Id, @OrganizationId, @UserId, N'BrandOwner');
    """,
    ("@Id", demoBrandOwnerMembershipId), ("@OrganizationId", demoOrganizationId), ("@UserId", demoOwnerId));

await transaction.CommitAsync();
Console.WriteLine($"Demo business ready. Sign in as {DemoOwnerEmail} with the password you passed.");
return 0;

async Task ExecuteAsync(string sql, params (string Name, object Value)[] parameters)
{
    await using var command = new SqlCommand(sql, connection, transaction);
    foreach (var (name, value) in parameters)
    {
        command.Parameters.AddWithValue(name, value);
    }

    await command.ExecuteNonQueryAsync();
}
