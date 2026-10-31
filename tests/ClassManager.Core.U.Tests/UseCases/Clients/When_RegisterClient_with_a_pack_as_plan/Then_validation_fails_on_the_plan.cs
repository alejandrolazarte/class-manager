using ClassManager.Core.Domain.ClassPacks;
using ClassManager.Core.UseCases.Clients;

namespace ClassManager.Core.U.Tests.UseCases.Clients.When_RegisterClient_with_a_pack_as_plan;

public sealed class Then_validation_fails_on_the_plan
{
    [Fact]
    public async Task Then_validation_fails_on_the_plan_Run()
    {
        var builder = new RegisterClientUseCaseBuilder();
        var pack = ClassPack.Create("4 clases", 4, 10000m, 1, TestData.Now).Value!.WithId();
        builder.ClassPacks.Setup(repository => repository.GetByIdAsync(pack.Id, It.IsAny<CancellationToken>())).ReturnsAsync(pack);
        var command = RegisterClientUseCaseBuilder.ValidCommand() with { PlanId = pack.Id };

        var response = await builder.Build().ExecuteAsync(command, CancellationToken.None);

        response.Error!.FieldName.ShouldBe(nameof(RegisterClientCommand.PlanId));
        builder.UnitOfWork.Verify(unitOfWork => unitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
