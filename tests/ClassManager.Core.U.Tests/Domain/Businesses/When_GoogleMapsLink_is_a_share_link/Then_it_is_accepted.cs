using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Core.U.Tests.Domain.Businesses.When_GoogleMapsLink_is_a_share_link;

public sealed class Then_it_is_accepted
{
    [Fact]
    public void Then_it_is_accepted_Run()
    {
        string[] links =
        [
            "https://maps.app.goo.gl/k3Hq8vPzR2",
            "https://goo.gl/maps/k3Hq8vPzR2",
            "https://www.google.com/maps/place/Escuela/@-34.6,-58.4,17z",
            "https://www.google.com.ar/maps/place/Escuela",
            "https://google.es/maps?q=Valencia",
            "https://maps.google.com/?q=Valencia",
        ];

        links.Where(link => !GoogleMapsLink.IsValid(link)).ShouldBeEmpty();
    }
}
