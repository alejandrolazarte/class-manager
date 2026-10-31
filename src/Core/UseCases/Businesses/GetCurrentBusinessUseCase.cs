using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Core.UseCases.Businesses;

public sealed record GetCurrentBusinessQuery : IQuery;

public sealed record BusinessResponse(
    string Name,
    string CountryCode,
    string TimeZoneId,
    string CurrencyCode,
    string DefaultCountryCallingCode,
    string? LocationName,
    string? LocationMapsUrl)
{
    public static BusinessResponse From(Business business) =>
        new(
            business.Name,
            business.CountryCode,
            business.TimeZoneId,
            business.CurrencyCode,
            business.DefaultCountryCallingCode,
            business.LocationName,
            business.LocationMapsUrl);
}

public sealed class GetCurrentBusinessUseCase(IBusinessRepository businessRepository)
    : IUseCase<GetCurrentBusinessQuery, BusinessResponse>
{
    public async Task<Result<BusinessResponse>> ExecuteAsync(GetCurrentBusinessQuery command, CancellationToken cancellationToken)
    {
        var business = await businessRepository.GetCurrentAsync(cancellationToken);
        if (business is null)
        {
            return Result.Unauthorized<BusinessResponse>(BusinessErrorCodes.CurrentBusinessNotFoundMessage, BusinessErrorCodes.CurrentBusinessNotFound);
        }

        return BusinessResponse.From(business);
    }
}
