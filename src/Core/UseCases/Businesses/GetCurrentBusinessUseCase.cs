using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Time;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Businesses;
using ClassManager.Core.Domain.Fees;
using ClassManager.Core.UseCases.Fees;

namespace ClassManager.Core.UseCases.Businesses;

public sealed record GetCurrentBusinessQuery : IQuery;

public sealed record BusinessResponse(
    string Name,
    string CountryCode,
    string TimeZoneId,
    string CurrencyCode,
    string DefaultCountryCallingCode,
    decimal? DefaultMonthlyFee,
    IReadOnlyList<MonthlyFeeChangeResponse> DefaultMonthlyFeeChanges,
    BusinessBillingMode? BillingMode)
{
    public static BusinessResponse From(Business business, IReadOnlyCollection<DefaultMonthlyFeeChange> feeChanges, DateOnly today) =>
        new(
            business.Name,
            business.CountryCode,
            business.TimeZoneId,
            business.CurrencyCode,
            business.DefaultCountryCallingCode,
            FeeTimeline.DefaultFeeIn(feeChanges, BillingMonth.From(today)),
            MonthlyFeeChangeResponse.From(feeChanges),
            FeeTimeline.BillingModeIn(feeChanges, BillingMonth.From(today)));
}

public sealed class GetCurrentBusinessUseCase(
    IBusinessRepository businessRepository,
    IFeeScheduleRepository feeScheduleRepository,
    IBusinessCalendarService businessCalendar)
    : IUseCase<GetCurrentBusinessQuery, BusinessResponse>
{
    public async Task<Result<BusinessResponse>> ExecuteAsync(GetCurrentBusinessQuery command, CancellationToken cancellationToken)
    {
        var business = await businessRepository.GetCurrentAsync(cancellationToken);
        if (business is null)
        {
            return Result.Unauthorized<BusinessResponse>(BusinessErrorCodes.CurrentBusinessNotFoundMessage, BusinessErrorCodes.CurrentBusinessNotFound);
        }

        var feeChanges = await feeScheduleRepository.ListDefaultFeeChangesAsync(cancellationToken);
        var today = await businessCalendar.TodayAsync(cancellationToken);
        return BusinessResponse.From(business, feeChanges, today);
    }
}
