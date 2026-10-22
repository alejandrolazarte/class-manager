using Microsoft.Extensions.Time.Testing;

namespace ClassManager.Backoffice.Core.U.Tests.Brands.When_add_ons_expire;

public sealed class Then_only_the_ones_ending_this_week_are_listed
{
    [Fact]
    public async Task Then_only_the_ones_ending_this_week_are_listed_Run()
    {
        var directory = new InMemoryBrandDirectory();
        var today = DateOnly.FromDateTime(BrandsScenario.Now.UtcDateTime);
        directory.ExpiringAddOns.Add(new ExpiringAddOn(TestIds.Next(), "Estudio", "shop", today.AddDays(6)));
        directory.ExpiringAddOns.Add(new ExpiringAddOn(TestIds.Next(), "Otra", "brand", today.AddDays(9)));

        var result = await new ListExpiringAddOnsUseCase(directory, new FakeTimeProvider(BrandsScenario.Now))
            .ExecuteAsync(new ListExpiringAddOnsQuery(), CancellationToken.None);

        result.Value!.AddOns.ShouldHaveSingleItem().BrandName.ShouldBe("Estudio");
    }
}
