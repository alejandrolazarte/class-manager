import { keepBreadcrumbWithoutPersonalData } from "@/errorReporting/errorReportPrivacy";

describe("When a breadcrumb comes from the console", () => {
  it("Then it is dropped", () => {
    const breadcrumb = keepBreadcrumbWithoutPersonalData({
      category: "console",
      message: "Saved student Ana",
    });

    expect(breadcrumb).toBeNull();
  });
});
