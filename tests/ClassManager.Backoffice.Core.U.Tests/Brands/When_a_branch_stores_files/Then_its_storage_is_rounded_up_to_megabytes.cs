namespace ClassManager.Backoffice.Core.U.Tests.Brands.When_a_branch_stores_files;

public sealed class Then_its_storage_is_rounded_up_to_megabytes
{
    private const long OneMegabyteAndOneByte = (1024 * 1024) + 1;

    [Fact]
    public async Task Then_its_storage_is_rounded_up_to_megabytes_Run()
    {
        var scenario = new BrandsScenario().WithBrand(
            [],
            branches: [new BrandBranch(Guid.CreateVersion7(), "Sede", "AR", BrandsScenario.Now, 3, 1, OneMegabyteAndOneByte)]);

        var brand = (await scenario.GetAsync()).Value!;

        brand.Branches.ShouldHaveSingleItem().StorageMegabytes.ShouldBe(2);
    }
}
