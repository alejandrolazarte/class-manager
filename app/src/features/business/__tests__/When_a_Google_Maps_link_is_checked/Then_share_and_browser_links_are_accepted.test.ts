import { isGoogleMapsLink } from "@/features/business/businessLocation";

describe("When a Google Maps link is checked", () => {
  it("Then share and browser links are accepted", () => {
    const links = [
      "https://maps.app.goo.gl/k3Hq8vPzR2",
      "https://goo.gl/maps/k3Hq8vPzR2",
      "https://www.google.com/maps/place/Escuela/@-34.6,-58.4,17z",
      "https://www.google.com.ar/maps/place/Escuela",
      "https://google.es/maps?q=Valencia",
      "https://maps.google.com/?q=Valencia",
    ];

    expect(links.filter((link) => !isGoogleMapsLink(link))).toEqual([]);
  });
});
