using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Core.UseCases.Brands;

public sealed record SetBrandLogoCommand(byte[] Content);

public sealed record RemoveBrandLogoCommand;

public sealed record GetBrandLogoQuery;

public sealed record BrandLogoFile(byte[] Content, string ContentType, DateTimeOffset UpdatedAt);

public sealed class SetBrandLogoUseCase(
    IBusinessRepository businessRepository,
    IBrandLogoRepository brandLogoRepository,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider)
    : IUseCase<SetBrandLogoCommand, BrandResponse>
{
    public async Task<Result<BrandResponse>> ExecuteAsync(SetBrandLogoCommand command, CancellationToken cancellationToken)
    {
        var business = await businessRepository.GetCurrentForUpdateAsync(cancellationToken);
        if (business is null)
        {
            return BrandRules.BusinessNotFound<BrandResponse>();
        }

        var newLogo = BrandLogo.Create(command.Content, timeProvider.GetUtcNow());
        if (newLogo.IsFailure)
        {
            return newLogo.Error!;
        }

        var currentLogo = await brandLogoRepository.GetForUpdateAsync(cancellationToken);
        if (currentLogo is null)
        {
            brandLogoRepository.Add(newLogo.Value!);
        }
        else
        {
            currentLogo.Replace(newLogo.Value!);
        }

        business.ChangeLogo(newLogo.Value!.UpdatedAt);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return BrandResponse.From(business);
    }
}

public sealed class RemoveBrandLogoUseCase(
    IBusinessRepository businessRepository,
    IBrandLogoRepository brandLogoRepository,
    IUnitOfWork unitOfWork)
    : IUseCase<RemoveBrandLogoCommand, BrandResponse>
{
    public async Task<Result<BrandResponse>> ExecuteAsync(RemoveBrandLogoCommand command, CancellationToken cancellationToken)
    {
        var business = await businessRepository.GetCurrentForUpdateAsync(cancellationToken);
        if (business is null)
        {
            return BrandRules.BusinessNotFound<BrandResponse>();
        }

        var currentLogo = await brandLogoRepository.GetForUpdateAsync(cancellationToken);
        if (currentLogo is not null)
        {
            brandLogoRepository.Remove(currentLogo);
        }

        business.RemoveLogo();
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return BrandResponse.From(business);
    }
}

public sealed class GetBrandLogoUseCase(IBrandLogoRepository brandLogoRepository) : IUseCase<GetBrandLogoQuery, BrandLogoFile>
{
    public async Task<Result<BrandLogoFile>> ExecuteAsync(GetBrandLogoQuery command, CancellationToken cancellationToken)
    {
        var logo = await brandLogoRepository.GetAsync(cancellationToken);

        return logo is null
            ? Result.NotFound<BrandLogoFile>(BrandErrorCodes.LogoNotFoundMessage, BrandErrorCodes.LogoNotFound)
            : new BrandLogoFile(logo.Content, logo.ContentType, logo.UpdatedAt);
    }
}
