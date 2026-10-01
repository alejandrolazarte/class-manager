import { findTabLinks, hasScreen, listTabNames, targetTabOf } from "@/testing/tabLinks";

describe("When a tab screen opens another screen", () => {
  it("Then the screen exists in that tab", () => {
    const tabNames = listTabNames();
    const tabLinks = findTabLinks().filter((link) => tabNames.includes(targetTabOf(link.path)));

    const linksWithoutScreen = tabLinks
      .filter((link) => !hasScreen(link.path))
      .map((link) => `${link.originTab} → ${link.path} (${link.sourceFile}: ${link.routeKey})`);

    expect(tabLinks.length).toBeGreaterThan(0);
    expect([...new Set(linksWithoutScreen)]).toEqual([]);
  });
});
