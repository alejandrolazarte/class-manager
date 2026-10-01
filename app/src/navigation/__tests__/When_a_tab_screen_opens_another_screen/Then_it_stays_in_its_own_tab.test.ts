import { findTabLinks, isTabRoot, listTabNames, targetTabOf } from "@/testing/tabLinks";

describe("When a tab screen opens another screen", () => {
  it("Then it stays in its own tab", () => {
    const tabNames = listTabNames();
    const tabLinks = findTabLinks();

    const crossTabLinks = tabLinks
      .filter((link) => tabNames.includes(targetTabOf(link.path)))
      .filter((link) => targetTabOf(link.path) !== link.originTab && !isTabRoot(link.path))
      .map((link) => `${link.originTab} → ${link.path} (${link.sourceFile}: ${link.routeKey})`);

    expect(tabLinks.length).toBeGreaterThan(0);
    expect([...new Set(crossTabLinks)]).toEqual([]);
  });
});
