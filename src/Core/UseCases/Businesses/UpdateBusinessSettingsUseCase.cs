using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Core.UseCases.Businesses;

public sealed record UpdateBusinessSettingsCommand(
    string? Name,
    string? TimeZoneId,
    string? CurrencyCode,
    string? DefaultCountryCallingCode,
    bool? NoticedAbsencesKeepStreak = null);

public sealed record UpdateBusinessSettingsResponse(
    string Name,
    string TimeZoneId,
    string CurrencyCode,
    string DefaultCountryCallingCode,
    bool NoticedAbsencesKeepStreak)
{
    public static UpdateBusinessSettingsResponse From(Business business) =>
        new(business.Name, business.TimeZoneId, business.CurrencyCode, business.DefaultCountryCallingCode, business.NoticedAbsencesKeepStreak);
}

public sealed class UpdateBusinessSettingsUseCase(
    IBusinessRepository businessRepository,
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

        var update = business.UpdateSettings(command.Name, command.TimeZoneId, command.CurrencyCode, command.DefaultCountryCallingCode, command.NoticedAbsencesKeepStreak);
        if (update.IsFailure)
        {
            return update.Error!;
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return UpdateBusinessSettingsResponse.From(business);
    }
}
