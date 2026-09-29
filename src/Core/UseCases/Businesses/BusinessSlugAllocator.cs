using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Core.UseCases.Businesses;

internal static class BusinessSlugAllocator
{
    private const int FirstSlugSuffix = 2;

    public static async Task<string> FindAvailableAsync(
        IBusinessRepository businessRepository,
        string? businessName,
        CancellationToken cancellationToken)
    {
        var baseSlug = BusinessSlug.FromName(businessName);
        var candidateSlug = baseSlug;

        for (var suffix = FirstSlugSuffix; await businessRepository.IsSlugTakenAsync(candidateSlug, cancellationToken); suffix++)
        {
            candidateSlug = BusinessSlug.WithSuffix(baseSlug, suffix);
        }

        return candidateSlug;
    }
}
