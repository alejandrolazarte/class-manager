using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Accounts;
using ClassManager.Core.Domain.Businesses;
using ClassManager.Core.Domain.Organizations;
using ClassManager.Core.UseCases.Businesses;

namespace ClassManager.Core.UseCases.Authentication;

internal sealed record NewBrand(Organization Organization, Business Business)
{
    public PendingBusiness Requested => new(Business.Name, Business.CountryCode, Business.TimeZoneId, Business.CurrencyCode);

    public static async Task<Result<NewBrand>> CreateAsync(
        string? businessName,
        string? countryCode,
        string? timeZoneId,
        string? currencyCode,
        IBusinessRepository businessRepository,
        ICountryRepository countryRepository,
        DateTimeOffset now,
        CancellationToken cancellationToken)
    {
        var organization = Organization.Create(businessName, now);
        if (organization.IsFailure)
        {
            return RenameField(organization.Error!, nameof(Organization.Name), nameof(RequestSignUpCommand.BusinessName));
        }

        var slug = await BusinessSlugAllocator.FindAvailableAsync(businessRepository, businessName, cancellationToken);
        var business = Business.CreateInNewOrganization(
            organization.Value!,
            businessName,
            slug,
            await countryRepository.FindAsync(countryCode, cancellationToken),
            timeZoneId,
            currencyCode,
            now);
        if (business.IsFailure)
        {
            return RenameField(business.Error!, nameof(Business.Name), nameof(RequestSignUpCommand.BusinessName));
        }

        return new NewBrand(organization.Value!, business.Value!);
    }

    public static ResultError RenameField(ResultError error, string entityFieldName, string commandFieldName) =>
        error.FieldName == entityFieldName ? error with { FieldName = commandFieldName } : error;
}
