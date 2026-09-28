import { isTabVisible, tabDefinitions } from "@/navigation/tabDefinitions";
import { coachPermissions } from "@/testing/memberFactory";

describe("When member lacks payments permission", () => {
  it("Then fees tab is hidden", () => {
    const visibleTabNames = tabDefinitions
      .filter((tab) => isTabVisible(tab, coachPermissions))
      .map((tab) => tab.name);

    expect(visibleTabNames).toEqual(["today", "classes", "students", "settings"]);
  });
});
