using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Time;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Businesses;
using ClassManager.Core.Domain.Fees;
using ClassManager.Core.UseCases.Businesses;

namespace ClassManager.Core.UseCases.Fees;

public sealed record DeleteDefaultMonthlyFeeChangeCommand(string EffectiveFrom) : ICommand;

public sealed class DeleteDefaultMonthlyFeeChangeUseCase(
    IBusinessRepository businessRepository,
    IFeeScheduleRepository feeScheduleRepository,
    IUnitOfWork unitOfWork,
    IBusinessCalendarService businessCalendar,
    TimeProvider timeProvider)
    : IUseCase<DeleteDefaultMonthlyFeeChangeCommand, BusinessResponse>
{
    private const string ChangeNotFoundMessage = "The business has no fee change for that month.";

    public async Task<Result<BusinessResponse>> ExecuteAsync(DeleteDefaultMonthlyFeeChangeCommand command, CancellationToken cancellationToken)
    {
        var business = await businessRepository.GetCurrentAsync(cancellationToken);
        if (business is null)
        {
            return Result.Unauthorized<BusinessResponse>(BusinessErrorCodes.CurrentBusinessNotFoundMessage, BusinessErrorCodes.CurrentBusinessNotFound);
        }

        var today = await businessCalendar.TodayAsync(cancellationToken);
        var effectiveFrom = UpcomingMonth.Resolve(command.EffectiveFrom, today, nameof(DeleteDefaultMonthlyFeeChangeCommand.EffectiveFrom));
        if (effectiveFrom.IsFailure)
        {
            return effectiveFrom.Error!;
        }

        var change = await feeScheduleRepository.FindDefaultFeeChangeForUpdateAsync(effectiveFrom.Value!.FirstDay, cancellationToken);
        if (change is null)
        {
            return Result.NotFound<BusinessResponse>(ChangeNotFoundMessage, FeeErrorCodes.FeeChangeNotFound);
        }

        change.Delete(timeProvider.GetUtcNow());
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var changes = await feeScheduleRepository.ListDefaultFeeChangesAsync(cancellationToken);
        return BusinessResponse.From(business, changes, today);
    }
}
