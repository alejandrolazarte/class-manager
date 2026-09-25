using ClassManager.Core.Domain.Clients;

namespace ClassManager.Core.UseCases.Clients;

public sealed record ClientResponse(
    Guid Id,
    string FullName,
    string PhoneNumber,
    string? Email,
    string? Notes,
    DateTimeOffset CreatedAt,
    decimal? MonthlyFee)
{
    public static ClientResponse From(Client client) =>
        new(client.Id, client.FullName, client.PhoneNumber.Value, client.Email, client.Notes, client.CreatedAt, client.MonthlyFee);
}
