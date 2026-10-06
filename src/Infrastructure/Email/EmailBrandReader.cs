using ClassManager.Infrastructure.Persistence;
using ClassManager.Notifications.Email;
using ClassManager.Tenancy.AspNetCore.Persistence;

namespace ClassManager.Infrastructure.Email;

internal sealed class EmailBrandReader(AppDbContext context)
{
    public async Task<EmailBrand?> ReadAsync(Guid? businessId, CancellationToken cancellationToken)
    {
        if (businessId is not { } id)
        {
            return null;
        }

        var business = await context.Businesses.AsNoTracking().IgnoreTenantFilter()
            .FirstOrDefaultAsync(candidate => candidate.Id == id, cancellationToken);
        if (business is null)
        {
            return null;
        }

        var logo = business.LogoUpdatedAt is null
            ? null
            : await context.BrandLogos.AsNoTracking().IgnoreTenantFilter()
                .Where(candidate => candidate.TenantId == id)
                .Select(candidate => new EmailInlineImage(EmailBrand.LogoContentId, candidate.Content, candidate.ContentType))
                .FirstOrDefaultAsync(cancellationToken);
        return new EmailBrand(business.BrandDisplayName, business.ThemeColor, business.AccentColor, logo);
    }
}
