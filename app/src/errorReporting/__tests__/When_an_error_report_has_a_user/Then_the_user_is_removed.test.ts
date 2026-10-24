import type { ErrorEvent } from "@sentry/react-native";
import { removePersonalData } from "@/errorReporting/errorReportPrivacy";

describe("When an error report has a user", () => {
  it("Then the user is removed", () => {
    const errorReport: ErrorEvent = {
      type: undefined,
      user: { email: "ana@example.com", ip_address: "203.0.113.7" },
    };

    const scrubbedReport = removePersonalData(errorReport);

    expect(scrubbedReport.user).toBeUndefined();
  });
});
