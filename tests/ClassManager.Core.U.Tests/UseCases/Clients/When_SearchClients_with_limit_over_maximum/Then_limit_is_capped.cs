using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.UseCases.Clients;

namespace ClassManager.Core.U.Tests.UseCases.Clients.When_SearchClients_with_limit_over_maximum;

public sealed class Then_limit_is_capped
{
    [Fact]
    public async Task Then_limit_is_capped_Run()
    {
        var businesses = new Mock<IBusinessRepository>();
        businesses.Setup(repository => repository.GetCurrentAsync(It.IsAny<CancellationToken>())).ReturnsAsync(TestData.Business());
        var clients = new Mock<IClientRepository>();
        clients.Setup(repository => repository.SearchAsync(It.IsAny<ClientSearchCriteria>(), It.IsAny<CancellationToken>())).ReturnsAsync([]);
        var useCase = new SearchClientsUseCase(businesses.Object, clients.Object);

        await useCase.ExecuteAsync(new SearchClientsQuery("ana", 500), CancellationToken.None);

        clients.Verify(repository => repository.SearchAsync(
            It.Is<ClientSearchCriteria>(criteria => criteria.Limit == SearchClientsUseCase.MaximumLimit),
            It.IsAny<CancellationToken>()));
    }
}
