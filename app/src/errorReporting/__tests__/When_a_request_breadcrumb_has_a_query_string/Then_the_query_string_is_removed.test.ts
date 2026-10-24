import { keepBreadcrumbWithoutPersonalData } from "@/errorReporting/errorReportPrivacy";

const requestAddress = "https://api.laztty.com/api/clients";

describe("When a request breadcrumb has a query string", () => {
  it("Then the query string is removed", () => {
    const breadcrumb = keepBreadcrumbWithoutPersonalData({
      category: "fetch",
      data: { method: "GET", url: `${requestAddress}?search=Ana`, status_code: 500 },
    });

    expect(breadcrumb?.data).toEqual({ method: "GET", url: requestAddress, status_code: 500 });
  });
});
