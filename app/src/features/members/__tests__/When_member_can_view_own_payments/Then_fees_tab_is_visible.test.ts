import { permissions } from "@/features/members/permissions";
import { isTabVisible, tabDefinitions } from "@/navigation/tabDefinitions";

describe("When member can view own payments", () => {
  it("Then fees tab is visible", () => {
    const feesTab = tabDefinitions.find((tab) => tab.name === "fees")!;

    expect(isTabVisible(feesTab, [permissions.paymentsViewOwn])).toBe(true);
  });
});
