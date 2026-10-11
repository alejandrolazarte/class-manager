#:package Microsoft.Data.SqlClient@6.1.1

using System.Globalization;
using Microsoft.Data.SqlClient;

const string ConnectionStringVariable = "BUSINESS_DATABASE";
const string DefaultCurrency = "EUR";
const string BrandOwnerRole = "BrandOwner";
const int CurrencyLength = 3;
const int NoteMaxLength = 200;
const string UsageMessage =
    """
    Usage: dotnet run scripts/set-subscription.cs -- <brand owner email | organization id> <plan> <price> [currency] [note]
      plan      free, lite, pro or enterprise (any code in billing.Plans)
      price     agreed monthly price, for example 0 or 19.50
      currency  ISO 4217 code, EUR when omitted or empty ("")
      note      why this price, for example "Own business"
    The connection string comes from the BUSINESS_DATABASE environment variable.
    """;

var connectionString = Environment.GetEnvironmentVariable(ConnectionStringVariable);
if (string.IsNullOrWhiteSpace(connectionString) || args.Length is < 3 or > 5)
{
    Console.Error.WriteLine(UsageMessage);
    return 1;
}

var subscriberArgument = args[0].Trim();
var planCode = args[1].Trim().ToLowerInvariant();
if (!decimal.TryParse(args[2], NumberStyles.Number, CultureInfo.InvariantCulture, out var price) || price < 0)
{
    Console.Error.WriteLine($"The price must be a number of 0 or more with a dot for decimals, not '{args[2]}'.");
    return 1;
}

var currencyArgument = args.ElementAtOrDefault(3);
var currency = (string.IsNullOrWhiteSpace(currencyArgument) ? DefaultCurrency : currencyArgument).Trim().ToUpperInvariant();
if (currency.Length != CurrencyLength || !currency.All(char.IsAsciiLetterUpper))
{
    Console.Error.WriteLine($"The currency must be an ISO 4217 code of {CurrencyLength} letters, not '{currency}'.");
    return 1;
}

var note = args.ElementAtOrDefault(4)?.Trim();
note = string.IsNullOrEmpty(note) ? null : note;
if (note is { Length: > NoteMaxLength })
{
    Console.Error.WriteLine($"The note must be at most {NoteMaxLength} characters.");
    return 1;
}

await using var connection = new SqlConnection(connectionString);
await connection.OpenAsync();
await using var transaction = (SqlTransaction)await connection.BeginTransactionAsync();

var organizationIds = Guid.TryParse(subscriberArgument, out var organizationIdArgument)
    ? await QueryGuidsAsync("SELECT Id FROM Organizations WHERE Id = @Id", ("@Id", organizationIdArgument))
    : await QueryGuidsAsync(
        """
        SELECT DISTINCT member.OrganizationId
        FROM OrganizationMembers member
        JOIN [identity].[AspNetUsers] person ON person.Id = member.UserId
        WHERE person.NormalizedEmail = UPPER(@Email) AND member.Role = @Role AND member.IsDeleted = 0
        """,
        ("@Email", subscriberArgument), ("@Role", BrandOwnerRole));

if (organizationIds.Count == 0)
{
    Console.Error.WriteLine($"No organization found for '{subscriberArgument}'. Pass the brand owner's email or the organization id.");
    return 1;
}

if (organizationIds.Count > 1)
{
    Console.Error.WriteLine($"'{subscriberArgument}' owns several organizations; pass one of their ids instead:");
    foreach (var candidateId in organizationIds)
    {
        Console.Error.WriteLine($"  {candidateId}  {await QueryOrganizationNameAsync(candidateId)}");
    }

    return 1;
}

var organizationId = organizationIds[0];
if (await QueryGuidsAsync("SELECT NEWID() FROM billing.Plans WHERE Code = @Code", ("@Code", planCode)) is { Count: 0 })
{
    Console.Error.WriteLine($"There is no plan '{planCode}' in billing.Plans.");
    return 1;
}

var now = DateTimeOffset.UtcNow;
await ExecuteAsync(
    "UPDATE billing.Subscriptions SET DeletedOn = @Now WHERE SubscriberId = @SubscriberId AND DeletedOn IS NULL",
    ("@Now", now), ("@SubscriberId", organizationId));

await ExecuteAsync(
    """
    INSERT INTO billing.Subscriptions (Id, SubscriberId, PlanCode, Price, Currency, Note, CreatedOn, DeletedOn, ExpiredOn)
    VALUES (@Id, @SubscriberId, @PlanCode, @Price, @Currency, @Note, @Now, NULL, NULL);
    """,
    ("@Id", Guid.CreateVersion7()), ("@SubscriberId", organizationId), ("@PlanCode", planCode), ("@Price", price),
    ("@Currency", currency), ("@Note", (object?)note ?? DBNull.Value), ("@Now", now));

var organizationName = await QueryOrganizationNameAsync(organizationId);
await transaction.CommitAsync();
Console.WriteLine(
    $"{organizationName} ({organizationId}) is now on '{planCode}' at {price.ToString(CultureInfo.InvariantCulture)} {currency} a month.");
Console.WriteLine("Add-ons belong to the previous subscription; add them again if this brand should keep them.");
return 0;

async Task ExecuteAsync(string sql, params (string Name, object Value)[] parameters)
{
    await using var command = CreateCommand(sql, parameters);
    await command.ExecuteNonQueryAsync();
}

async Task<List<Guid>> QueryGuidsAsync(string sql, params (string Name, object Value)[] parameters)
{
    await using var command = CreateCommand(sql, parameters);
    await using var reader = await command.ExecuteReaderAsync();
    var ids = new List<Guid>();
    while (await reader.ReadAsync())
    {
        ids.Add(reader.GetGuid(0));
    }

    return ids;
}

async Task<string> QueryOrganizationNameAsync(Guid id)
{
    await using var command = CreateCommand("SELECT Name FROM Organizations WHERE Id = @Id", ("@Id", id));
    return (string)(await command.ExecuteScalarAsync())!;
}

SqlCommand CreateCommand(string sql, params (string Name, object Value)[] parameters)
{
    var command = new SqlCommand(sql, connection, transaction);
    foreach (var (name, value) in parameters)
    {
        command.Parameters.AddWithValue(name, value);
    }

    return command;
}
