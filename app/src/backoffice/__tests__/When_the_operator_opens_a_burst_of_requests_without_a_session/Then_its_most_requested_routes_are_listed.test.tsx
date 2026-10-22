import { fireEvent, screen } from "@testing-library/react-native";
import { getCommandLog, getSecurityEvents } from "@/backoffice/logs/logsApi";
import { LogsScreen } from "@/backoffice/logs/LogsScreen";
import { translate } from "@/i18n/translate";
import { aSecurityEventEntry } from "@/testing/backofficeLogs";
import { renderBackofficeScreen } from "@/testing/renderBackofficeScreen";

jest.mock("@/backoffice/logs/logsApi", () => ({
  ...jest.requireActual("@/backoffice/logs/logsApi"),
  getCommandLog: jest.fn(),
  getSecurityEvents: jest.fn(),
}));

const burst = aSecurityEventEntry({
  event: "UnauthorizedBurst",
  route: null,
  method: null,
  statusCode: null,
  count: 1070,
  traceId: null,
  routes: [
    { method: "GET", route: "/api/students", count: 300 },
    { method: "GET", route: "/api/classes", count: 250 },
    { method: "GET", route: "/api/payments", count: 120 },
    { method: "POST", route: "/api/enrollments", count: 80 },
    { method: "GET", route: "/api/fees", count: 60 },
    { method: "GET", route: "/api/branches", count: 50 },
  ],
  otherRouteRequests: 210,
});

describe("When the operator opens a burst of requests without a session", () => {
  it("Then its most requested routes are listed", async () => {
    jest
      .mocked(getCommandLog)
      .mockResolvedValue({ day: "2026-10-21", entries: [], isTruncated: false, isConnected: true });
    jest
      .mocked(getSecurityEvents)
      .mockResolvedValue({ day: "2026-10-21", events: [burst], isConnected: true });
    await renderBackofficeScreen(<LogsScreen />);
    await screen.findByText(translate("backoffice.logs.noCommands"));
    await fireEvent.press(screen.getByText(translate("backoffice.logs.securityEvents")));

    await fireEvent.press(
      await screen.findByText(translate("backoffice.logs.event.UnauthorizedBurst")),
    );

    expect(
      screen.getByText(
        [
          `GET /api/students · ${translate("backoffice.logs.times", { count: 300 })}`,
          `GET /api/classes · ${translate("backoffice.logs.times", { count: 250 })}`,
          `GET /api/payments · ${translate("backoffice.logs.times", { count: 120 })}`,
          `POST /api/enrollments · ${translate("backoffice.logs.times", { count: 80 })}`,
          `GET /api/fees · ${translate("backoffice.logs.times", { count: 60 })}`,
          `${translate("backoffice.logs.otherRoutes")} · ${translate("backoffice.logs.times", { count: 260 })}`,
        ].join("\n"),
      ),
    ).toBeOnTheScreen();
  });
});
