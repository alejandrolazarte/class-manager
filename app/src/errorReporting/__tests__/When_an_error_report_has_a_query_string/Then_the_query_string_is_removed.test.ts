import type { ErrorEvent } from "@sentry/react-native";
import { removePersonalData } from "@/errorReporting/errorReportPrivacy";

const pageAddress = "https://app.laztty.com/reset-password";

describe("When an error report has a query string", () => {
  it("Then the query string is removed", () => {
    const errorReport: ErrorEvent = {
      type: undefined,
      request: {
        url: `${pageAddress}?token=secret`,
        query_string: "token=secret",
        headers: { Referer: `${pageAddress}?token=secret` },
      },
    };

    const scrubbedReport = removePersonalData(errorReport);

    expect(scrubbedReport.request).toEqual({ url: pageAddress });
  });
});
