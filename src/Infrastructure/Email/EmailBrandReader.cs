using ClassManager.Infrastructure.Persistence;

namespace ClassManager.Infrastructure.Email;

internal sealed class EmailBrandReader(AppDbContext context)
{
    public async Task<EmailBrand> ReadAsync(Guid? businessId, CancellationToken cancellationToken)
    {
        if (businessId is not { } id)
        {
            return EmailBrand.App;
        }

        var business = await context.Businesses.AsNoTracking().IgnoreQueryFilters()
            .FirstOrDefaultAsync(candidate => candidate.Id == id, cancellationToken);
        if (business is null)
        {
            return EmailBrand.App;
        }

        var logo = business.LogoUpdatedAt is null
            ? null
            : await context.BrandLogos.AsNoTracking().IgnoreQueryFilters()
                .Where(candidate => candidate.TenantId == id)
                .Select(candidate => new EmailInlineImage(EmailBrand.LogoContentId, candidate.Content, candidate.ContentType))
                .FirstOrDefaultAsync(cancellationToken);
        return new EmailBrand(business.BrandDisplayName, business.ThemeColor, business.AccentColor, logo, IsBusiness: true);
    }
}
