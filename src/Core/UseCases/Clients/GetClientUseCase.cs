using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Clients;

namespace ClassManager.Core.UseCases.Clients;

public sealed record GetClientQuery(Guid ClientId);

public sealed class GetClientUseCase(IClientRepository clientRepository, IStudentRepository studentRepository)
    : IUseCase<GetClientQuery, ClientDetailsResponse>
{
    private const string NotFoundMessage = "The client does not exist.";

    public async Task<Result<ClientDetailsResponse>> ExecuteAsync(GetClientQuery command, CancellationToken cancellationToken)
    {
        var client = await clientRepository.GetByIdAsync(command.ClientId, cancellationToken);
        if (client is null)
        {
            return Result.NotFound<ClientDetailsResponse>(NotFoundMessage, ClientErrorCodes.NotFound);
        }

        var students = await studentRepository.ListByClientAsync(client.Id, cancellationToken);

        return ClientDetailsResponse.From(client, students);
    }
}
