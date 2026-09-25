using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Core.U.Tests.Domain.Businesses.When_BusinessSlug_gets_a_numeric_suffix;

public sealed class Then_it_fits_the_maximum_length
{
    [Fact]
    public void Then_it_fits_the_maximum_length_Run()
    {
        var baseSlug = BusinessSlug.FromName(new string('a', Business.NameMaxLength));

        var slug = BusinessSlug.WithSuffix(baseSlug, 12);

        slug.Length.ShouldBeLessThanOrEqualTo(Business.SlugMaxLength);
        slug.ShouldEndWith("-12");
    }
}
