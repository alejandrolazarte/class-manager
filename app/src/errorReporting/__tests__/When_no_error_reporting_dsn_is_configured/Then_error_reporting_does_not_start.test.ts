import * as Sentry from "@sentry/react-native";
import { startErrorReporting } from "@/errorReporting/startErrorReporting";

jest.mock("@sentry/react-native", () => ({ init: jest.fn() }));

describe("When no error reporting dsn is configured", () => {
  it("Then error reporting does not start", () => {
    delete process.env.EXPO_PUBLIC_SENTRY_DSN;

    startErrorReporting();

    expect(Sentry.init).not.toHaveBeenCalled();
  });
});
