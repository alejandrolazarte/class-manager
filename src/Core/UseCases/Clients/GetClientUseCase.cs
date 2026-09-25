using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Clients;

namespace ClassManager.Core.UseCases.Clients;

public sealed record GetClientQuery(Guid ClientId);

public sealed class GetClientUseCase(IClientRepository clientRepository)
    : IUseCase<GetClientQuery, ClientResponse>
{
    private const string NotFoundMessage = "The client does not exist.";

    public async Task<Result<ClientResponse>> ExecuteAsync(GetClientQuery command, CancellationToken cancellationToken)
    {
        var client = await clientRepository.GetByIdAsync(command.ClientId, cancellationToken);
        if (client is null)
        {
            return Result.NotFound<ClientResponse>(NotFoundMessage, ClientErrorCodes.NotFound);
        }

        return ClientResponse.From(client);
    }
}
