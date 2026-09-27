using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Time;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Businesses;
using ClassManager.Core.Domain.Fees;
using ClassManager.Core.UseCases.Businesses;

namespace ClassManager.Core.UseCases.Fees;

public sealed record SetDefaultMonthlyFeeCommand(decimal? Amount, string? EffectiveFrom);

public sealed class SetDefaultMonthlyFeeUseCase(
    IBusinessRepository businessRepository,
    IFeeScheduleRepository feeScheduleRepository,
    IUnitOfWork unitOfWork,
    IBusinessCalendarService businessCalendar,
    TimeProvider timeProvider)
    : IUseCase<SetDefaultMonthlyFeeCommand, BusinessResponse>
{
    public async Task<Result<BusinessResponse>> ExecuteAsync(SetDefaultMonthlyFeeCommand command, CancellationToken cancellationToken)
    {
        var business = await businessRepository.GetCurrentAsync(cancellationToken);
        if (business is null)
        {
            return Result.Unauthorized<BusinessResponse>(BusinessErrorCodes.CurrentBusinessNotFoundMessage, BusinessErrorCodes.CurrentBusinessNotFound);
        }

        var today = await businessCalendar.TodayAsync(cancellationToken);
        var effectiveFrom = EffectiveMonth.Resolve(command.EffectiveFrom, today, nameof(SetDefaultMonthlyFeeCommand.EffectiveFrom));
        if (effectiveFrom.IsFailure)
        {
            return effectiveFrom.Error!;
        }

        var change = DefaultMonthlyFeeChange.Create(effectiveFrom.Value!, command.Amount, today, timeProvider.GetUtcNow());
        if (change.IsFailure)
        {
            return change.Error!;
        }

        var existingChange = await feeScheduleRepository.FindDefaultFeeChangeForUpdateAsync(change.Value!.EffectiveFrom, cancellationToken);
        if (existingChange is null)
        {
            feeScheduleRepository.Add(change.Value);
        }
        else
        {
            existingChange.ReplaceWith(change.Value);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        var changes = await feeScheduleRepository.ListDefaultFeeChangesAsync(cancellationToken);
        return BusinessResponse.From(business, changes, today);
    }
}
