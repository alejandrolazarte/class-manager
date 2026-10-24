import { keepBreadcrumbWithoutPersonalData } from "@/errorReporting/errorReportPrivacy";

describe("When a breadcrumb comes from a tap", () => {
  it("Then it is dropped", () => {
    const breadcrumb = keepBreadcrumbWithoutPersonalData({
      category: "ui.click",
      message: 'div[aria-label="Llamar a Ana"]',
    });

    expect(breadcrumb).toBeNull();
  });
});
