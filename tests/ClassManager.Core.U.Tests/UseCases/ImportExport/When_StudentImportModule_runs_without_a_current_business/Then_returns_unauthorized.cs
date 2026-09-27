using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Core.U.Tests.UseCases.ImportExport.When_StudentImportModule_runs_without_a_current_business;

public sealed class Then_returns_unauthorized
{
    [Fact]
    public async Task Then_returns_unauthorized_Run()
    {
        var builder = new StudentImportBuilder();
        builder.Businesses.Setup(repository => repository.GetCurrentAsync(It.IsAny<CancellationToken>())).ReturnsAsync((Business?)null);

        var plan = await builder.PlanAsync("Ana Pérez;11 5555-6666;;;\n");

        plan.Error!.Code.ShouldBe(BusinessErrorCodes.CurrentBusinessNotFound);
    }
}
