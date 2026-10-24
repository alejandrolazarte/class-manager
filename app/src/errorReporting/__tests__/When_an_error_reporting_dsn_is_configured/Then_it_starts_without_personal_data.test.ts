import * as Sentry from "@sentry/react-native";
import { startErrorReporting } from "@/errorReporting/startErrorReporting";

jest.mock("@sentry/react-native", () => ({ init: jest.fn() }));

const dsn = "https://public-key@o1.ingest.de.sentry.io/2";

describe("When an error reporting dsn is configured", () => {
  it("Then it starts without personal data", () => {
    process.env.EXPO_PUBLIC_SENTRY_DSN = dsn;

    startErrorReporting();

    expect(Sentry.init).toHaveBeenCalledWith(
      expect.objectContaining({ dsn, sendDefaultPii: false }),
    );
  });
});
