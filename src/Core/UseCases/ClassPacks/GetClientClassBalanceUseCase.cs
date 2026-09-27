using ClassManager.Core.Abstractions.Fees;
using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Clients;

namespace ClassManager.Core.UseCases.ClassPacks;

public sealed record GetClientClassBalanceQuery(Guid ClientId);

public sealed class GetClientClassBalanceUseCase(IClientRepository clientRepository, IClassBalanceService classBalanceService)
    : IUseCase<GetClientClassBalanceQuery, ClassBalanceResponse>
{
    private const string ClientNotFoundMessage = "The client does not exist.";

    public async Task<Result<ClassBalanceResponse>> ExecuteAsync(GetClientClassBalanceQuery command, CancellationToken cancellationToken)
    {
        var client = await clientRepository.GetByIdAsync(command.ClientId, cancellationToken);
        if (client is null)
        {
            return Result.NotFound<ClassBalanceResponse>(ClientNotFoundMessage, ClientErrorCodes.NotFound);
        }

        var balances = await classBalanceService.CalculateAsync([client.Id], cancellationToken);
        return ClassBalanceResponse.From(balances[client.Id]);
    }
}
