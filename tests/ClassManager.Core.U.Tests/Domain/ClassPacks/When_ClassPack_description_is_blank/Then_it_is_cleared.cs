using ClassManager.Core.Domain.ClassPacks;

namespace ClassManager.Core.U.Tests.Domain.ClassPacks.When_ClassPack_description_is_blank;

public sealed class Then_it_is_cleared
{
    [Fact]
    public void Then_it_is_cleared_Run()
    {
        var pack = ClassPack.Create("8 clases", 8, 160m, 2, TestData.Now).Value!;
        pack.Describe("Para quienes empiezan");

        pack.Describe("   ");

        pack.Description.ShouldBeNull();
    }
}
