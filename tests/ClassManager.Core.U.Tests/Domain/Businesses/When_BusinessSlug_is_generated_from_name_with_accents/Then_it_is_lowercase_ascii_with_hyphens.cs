using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Core.U.Tests.Domain.Businesses.When_BusinessSlug_is_generated_from_name_with_accents;

public sealed class Then_it_is_lowercase_ascii_with_hyphens
{
    [Fact]
    public void Then_it_is_lowercase_ascii_with_hyphens_Run()
    {
        var slug = BusinessSlug.FromName("  Panadería   Laura & Co.  ");

        slug.ShouldBe("panaderia-laura-co");
    }
}
