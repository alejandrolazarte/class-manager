using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Domain.Businesses;
using ClassManager.Core.UseCases.Businesses;

namespace ClassManager.Core.U.Tests.UseCases.Businesses;

internal sealed class UpdateBusinessSettingsUseCaseBuilder
{
    public Business Business { get; } = TestData.Business();

    public Mock<IBusinessRepository> Businesses { get; } = new();
    public Mock<IUnitOfWork> UnitOfWork { get; } = new();

    public UpdateBusinessSettingsUseCaseBuilder()
    {
        Businesses.Setup(repository => repository.GetCurrentForUpdateAsync(It.IsAny<CancellationToken>())).ReturnsAsync(Business);
    }

    public UpdateBusinessSettingsCommand CommandFor(string countryCode, string timeZoneId) =>
        new(Business.Name, countryCode, timeZoneId, Business.CurrencyCode);

    public UpdateBusinessSettingsUseCase Build() =>
        new(Businesses.Object, new InMemoryCountryRepository(), UnitOfWork.Object);

}
