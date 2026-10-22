using ClassManager.Core.Domain.Clients;

namespace ClassManager.Core.UseCases.Clients;

public sealed record ClientResponse(
    long Id,
    string FullName,
    string PhoneNumber,
    string? Email,
    string? Notes,
    DateTimeOffset CreatedAt)
{
    public static ClientResponse From(Client client) =>
        new(client.Id, client.User.FullName, client.User.PhoneNumber!.Value, client.User.Email, client.Notes, client.CreatedAt);
}
