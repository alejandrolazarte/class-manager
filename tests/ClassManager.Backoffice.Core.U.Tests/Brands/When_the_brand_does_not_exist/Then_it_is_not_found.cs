using ClassManager.Core.Common;

namespace ClassManager.Backoffice.Core.U.Tests.Brands.When_the_brand_does_not_exist;

public sealed class Then_it_is_not_found
{
    [Fact]
    public async Task Then_it_is_not_found_Run()
    {
        var scenario = new BrandsScenario();

        var result = await scenario.GetAsync(TestIds.Next());

        result.Error!.Kind.ShouldBe(ErrorKind.NotFound);
    }
}
