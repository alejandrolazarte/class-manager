using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Clients;
using ClassManager.Core.UseCases.Clients;

namespace ClassManager.Core.UseCases.Fees;

public sealed record SetClientMonthlyFeeCommand(Guid ClientId, decimal? Amount);

public sealed class SetClientMonthlyFeeUseCase(IClientRepository clientRepository, IUnitOfWork unitOfWork)
    : IUseCase<SetClientMonthlyFeeCommand, ClientResponse>
{
    private const string ClientNotFoundMessage = "The client does not exist.";

    public async Task<Result<ClientResponse>> ExecuteAsync(SetClientMonthlyFeeCommand command, CancellationToken cancellationToken)
    {
        var client = await clientRepository.GetForUpdateAsync(command.ClientId, cancellationToken);
        if (client is null)
        {
            return Result.NotFound<ClientResponse>(ClientNotFoundMessage, ClientErrorCodes.NotFound);
        }

        var update = client.SetMonthlyFee(command.Amount);
        if (update.IsFailure)
        {
            return update.Error! with { FieldName = nameof(SetClientMonthlyFeeCommand.Amount) };
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ClientResponse.From(client);
    }
}
