using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Core.UseCases.Businesses;

public sealed record GetCurrentBusinessQuery;

public sealed record BusinessResponse(
    string Name,
    string TimeZoneId,
    string CurrencyCode,
    string DefaultCountryCallingCode)
{
    public static BusinessResponse From(Business business) =>
        new(business.Name, business.TimeZoneId, business.CurrencyCode, business.DefaultCountryCallingCode);
}

public sealed class GetCurrentBusinessUseCase(IBusinessRepository businessRepository) : IUseCase<GetCurrentBusinessQuery, BusinessResponse>
{
    public async Task<Result<BusinessResponse>> ExecuteAsync(GetCurrentBusinessQuery command, CancellationToken cancellationToken)
    {
        var business = await businessRepository.GetCurrentAsync(cancellationToken);

        return business is null
            ? Result.Unauthorized<BusinessResponse>(BusinessErrorCodes.CurrentBusinessNotFoundMessage, BusinessErrorCodes.CurrentBusinessNotFound)
            : BusinessResponse.From(business);
    }
}
