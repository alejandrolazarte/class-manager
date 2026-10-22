import { isGoogleMapsLink } from "@/features/business/businessLocation";

describe("When a link is not from Google Maps", () => {
  it("Then it is rejected", () => {
    const links = [
      "http://maps.app.goo.gl/k3Hq8vPzR2",
      "https://www.google.com/search?q=sede",
      "https://goo.gl/k3Hq8vPzR2",
      "https://maps.app.goo.gl.example.com/k3Hq8vPzR2",
      "https://www.google.example.com/maps",
      "www.escueladelparque.com",
      "Av. Costanera 1450",
    ];

    expect(links.filter(isGoogleMapsLink)).toEqual([]);
  });
});
