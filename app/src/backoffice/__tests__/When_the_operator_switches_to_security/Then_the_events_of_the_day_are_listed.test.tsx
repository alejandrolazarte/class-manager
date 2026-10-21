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

describe("When the operator switches to security", () => {
  it("Then the events of the day are listed", async () => {
    jest
      .mocked(getCommandLog)
      .mockResolvedValue({ day: "2026-10-21", entries: [], isTruncated: false });
    jest
      .mocked(getSecurityEvents)
      .mockResolvedValue({ day: "2026-10-21", events: [aSecurityEventEntry()] });
    await renderBackofficeScreen(<LogsScreen />);
    await screen.findByText(translate("backoffice.logs.noCommands"));

    await fireEvent.press(screen.getByText(translate("backoffice.logs.securityEvents")));

    expect(
      await screen.findByText(translate("backoffice.logs.event.SignInFailed")),
    ).toBeOnTheScreen();
    expect(screen.getByText("09:15 · POST /api/auth/signin 401 · 203.0.113.7")).toBeOnTheScreen();
  });
});
