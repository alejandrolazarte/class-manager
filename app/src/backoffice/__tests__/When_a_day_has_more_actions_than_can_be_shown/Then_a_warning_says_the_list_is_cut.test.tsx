import { screen } from "@testing-library/react-native";
import { getCommandLog } from "@/backoffice/logs/logsApi";
import { LogsScreen } from "@/backoffice/logs/LogsScreen";
import { translate } from "@/i18n/translate";
import { aCommandLogEntry } from "@/testing/backofficeLogs";
import { renderBackofficeScreen } from "@/testing/renderBackofficeScreen";

jest.mock("@/backoffice/logs/logsApi", () => ({
  ...jest.requireActual("@/backoffice/logs/logsApi"),
  getCommandLog: jest.fn(),
}));

describe("When a day has more actions than can be shown", () => {
  it("Then a warning says the list is cut", async () => {
    jest.mocked(getCommandLog).mockResolvedValue({
      day: "2026-10-21",
      entries: [aCommandLogEntry()],
      isTruncated: true,
      isConnected: true,
    });

    await renderBackofficeScreen(<LogsScreen />);

    expect(await screen.findByText(translate("backoffice.logs.truncated"))).toBeOnTheScreen();
  });
});
