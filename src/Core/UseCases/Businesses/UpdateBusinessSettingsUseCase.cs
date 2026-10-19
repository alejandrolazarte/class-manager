using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Audit;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Core.UseCases.Businesses;

public sealed record UpdateBusinessSettingsCommand(
    [property: AuditPlain] string? Name,
    [property: AuditPlain] string? CountryCode,
    [property: AuditPlain] string? TimeZoneId,
    [property: AuditPlain] string? CurrencyCode) : ICommand;

public sealed record UpdateBusinessSettingsResponse(
    string Name,
    string CountryCode,
    string TimeZoneId,
    string CurrencyCode,
    string DefaultCountryCallingCode)
{
    public static UpdateBusinessSettingsResponse From(Business business) =>
        new(business.Name, business.CountryCode, business.TimeZoneId, business.CurrencyCode, business.DefaultCountryCallingCode);
}

public sealed class UpdateBusinessSettingsUseCase(
    IBusinessRepository businessRepository,
    ICountryRepository countryRepository,
    IUnitOfWork unitOfWork)
    : IUseCase<UpdateBusinessSettingsCommand, UpdateBusinessSettingsResponse>
{
    public async Task<Result<UpdateBusinessSettingsResponse>> ExecuteAsync(UpdateBusinessSettingsCommand command, CancellationToken cancellationToken)
    {
        var business = await businessRepository.GetCurrentForUpdateAsync(cancellationToken);
        if (business is null)
        {
            return Result.Unauthorized<UpdateBusinessSettingsResponse>(BusinessErrorCodes.CurrentBusinessNotFoundMessage, BusinessErrorCodes.CurrentBusinessNotFound);
        }

        var country = await countryRepository.FindAsync(command.CountryCode, cancellationToken);
        var update = business.UpdateSettings(command.Name, country, command.TimeZoneId, command.CurrencyCode);
        if (update.IsFailure)
        {
            return update.Error!;
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return UpdateBusinessSettingsResponse.From(business);
    }
}
