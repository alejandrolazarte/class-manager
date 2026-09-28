using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.UseCases.Clients;

namespace ClassManager.Core.U.Tests.UseCases.Clients.When_SearchClients_with_phone_digits;

public sealed class Then_search_uses_normalized_prefix
{
    [Fact]
    public async Task Then_search_uses_normalized_prefix_Run()
    {
        var businesses = new Mock<IBusinessRepository>();
        businesses.Setup(repository => repository.GetCurrentAsync(It.IsAny<CancellationToken>())).ReturnsAsync(TestData.Business());
        var clients = new Mock<IClientRepository>();
        clients.Setup(repository => repository.SearchAsync(It.IsAny<ClientSearchCriteria>(), It.IsAny<CancellationToken>())).ReturnsAsync([]);
        var useCase = new SearchClientsUseCase(businesses.Object, clients.Object, new EveryAccessScopes());

        await useCase.ExecuteAsync(new SearchClientsQuery("11 22", null), CancellationToken.None);

        clients.Verify(repository => repository.SearchAsync(
            It.Is<ClientSearchCriteria>(criteria => criteria.PhoneNumberPrefix == "+541122"),
            It.IsAny<CancellationToken>()));
    }
}
