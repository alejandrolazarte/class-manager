using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Businesses;
using ClassManager.Core.Domain.Clients;

namespace ClassManager.Core.UseCases.Clients;

public sealed record SearchClientsQuery(string? Search, int? Limit);

public sealed class SearchClientsUseCase(
    IBusinessRepository businessRepository,
    IClientRepository clientRepository)
    : IUseCase<SearchClientsQuery, IReadOnlyList<ClientResponse>>
{
    public const int DefaultLimit = 20;
    public const int MaximumLimit = 50;
    public const int MinimumLimit = 1;

    public async Task<Result<IReadOnlyList<ClientResponse>>> ExecuteAsync(SearchClientsQuery command, CancellationToken cancellationToken)
    {
        var business = await businessRepository.GetCurrentAsync(cancellationToken);
        if (business is null)
        {
            return Result.Unauthorized<IReadOnlyList<ClientResponse>>(BusinessErrorCodes.CurrentBusinessNotFoundMessage, BusinessErrorCodes.CurrentBusinessNotFound);
        }

        var search = string.IsNullOrWhiteSpace(command.Search) ? null : command.Search.Trim();
        var criteria = new ClientSearchCriteria(
            search,
            PhoneNumber.ToNormalizedPrefix(search, business.DefaultCountryCallingCode),
            Math.Clamp(command.Limit ?? DefaultLimit, MinimumLimit, MaximumLimit));

        var clients = await clientRepository.SearchAsync(criteria, cancellationToken);

        return Result.Success<IReadOnlyList<ClientResponse>>([.. clients.Select(ClientResponse.From)]);
    }
}
