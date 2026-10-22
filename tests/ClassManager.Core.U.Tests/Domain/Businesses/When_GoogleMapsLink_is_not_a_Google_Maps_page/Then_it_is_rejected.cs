using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Core.U.Tests.Domain.Businesses.When_GoogleMapsLink_is_not_a_Google_Maps_page;

public sealed class Then_it_is_rejected
{
    [Fact]
    public void Then_it_is_rejected_Run()
    {
        string[] links =
        [
            "http://maps.app.goo.gl/k3Hq8vPzR2",
            "https://www.google.com/search?q=sede",
            "https://goo.gl/k3Hq8vPzR2",
            "https://maps.app.goo.gl.example.com/k3Hq8vPzR2",
            "https://www.google.example.com/maps",
            "www.escueladelparque.com",
            "Av. Costanera 1450",
        ];

        links.Where(GoogleMapsLink.IsValid).ShouldBeEmpty();
    }
}
