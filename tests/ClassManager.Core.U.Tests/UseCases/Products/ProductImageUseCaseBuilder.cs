using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Storage;
using ClassManager.Core.Domain.Images;
using ClassManager.Core.Domain.Products;
using ClassManager.Core.UseCases.Products;

namespace ClassManager.Core.U.Tests.UseCases.Products;

internal sealed class ProductImageUseCaseBuilder
{
    public static readonly Uri NewImageUrl = new("https://files.example.com/public-files/tenant/products/new.png");

    public ProductImageUseCaseBuilder()
    {
        ProductRepository.Setup(repository => repository.GetForUpdateAsync(Product.Id, It.IsAny<CancellationToken>())).ReturnsAsync(Product);
        StockMovementRepository
            .Setup(repository => repository.StockByVariantAsync(It.IsAny<IReadOnlyCollection<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Dictionary<Guid, int>());
        CatalogImageService
            .Setup(service => service.SaveAsync(CatalogImageOwner.Product, Product.Id, It.IsAny<CatalogImage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(NewImageUrl);
    }

    public Product Product { get; } = Product.Create("Gorro", null, 12m, StockMode.Unlimited, true, [new VariantChange(null, null)], TestData.Now).Value!;

    public Mock<IProductRepository> ProductRepository { get; } = new();

    public Mock<IStockMovementRepository> StockMovementRepository { get; } = new();

    public Mock<ICatalogImageService> CatalogImageService { get; } = new();

    public Mock<IUnitOfWork> UnitOfWork { get; } = new();

    public SetProductImageUseCase BuildSetUseCase() =>
        new(ProductRepository.Object, StockMovementRepository.Object, CatalogImageService.Object, UnitOfWork.Object);

    public RemoveProductImageUseCase BuildRemoveUseCase() =>
        new(ProductRepository.Object, StockMovementRepository.Object, CatalogImageService.Object, UnitOfWork.Object);
}
