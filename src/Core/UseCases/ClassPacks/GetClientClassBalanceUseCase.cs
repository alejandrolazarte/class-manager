using ClassManager.Core.Abstractions.Fees;
using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Clients;

namespace ClassManager.Core.UseCases.ClassPacks;

public sealed record GetClientClassBalanceQuery(Guid ClientId);

public sealed class GetClientClassBalanceUseCase(
    IClientRepository clientRepository,
    IClassBalanceService classBalanceService,
    IPrivateLessonRepository privateLessonRepository,
    IClassPackPurchaseRepository purchaseRepository)
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
        var deductedTrialIds = (await purchaseRepository.ListByClientsAsync([client.Id], cancellationToken))
            .Select(purchase => purchase.TrialLessonId)
            .OfType<Guid>()
            .ToHashSet();
        IReadOnlyList<DeductibleTrialResponse> deductibleTrials =
        [
            .. (await privateLessonRepository.ListPaidTrialsByClientAsync(client.Id, cancellationToken))
                .Where(trial => !deductedTrialIds.Contains(trial.PrivateLessonId))
                .DistinctBy(trial => trial.PrivateLessonId)
                .Select(trial => new DeductibleTrialResponse(trial.PrivateLessonId, trial.Date, trial.StudentFullName, trial.TrialPrice)),
        ];
        return ClassBalanceResponse.From(balances[client.Id], deductibleTrials);
    }
}
