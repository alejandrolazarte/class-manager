using ClassManager.Core.UseCases.ClassPacks;

namespace ClassManager.Core.U.Tests.UseCases.ClassPacks.When_CreateClassPack_name_is_taken;

public sealed class Then_returns_conflict
{
    [Fact]
    public async Task Then_returns_conflict_Run()
    {
        var builder = new ClassPackUseCaseBuilder();
        builder.ClassPacks.Setup(repository => repository.FindByNameAsync(builder.Pack.Name, It.IsAny<CancellationToken>())).ReturnsAsync(builder.Pack);

        var response = await builder.BuildCreate().ExecuteAsync(
            new CreateClassPackCommand(builder.Pack.Name, 8, 160m, 2), CancellationToken.None);

        response.Error!.Kind.ShouldBe(ErrorKind.Conflict);
    }
}
