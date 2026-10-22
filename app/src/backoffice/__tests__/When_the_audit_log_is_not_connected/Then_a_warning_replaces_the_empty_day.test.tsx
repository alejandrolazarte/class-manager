import { screen } from "@testing-library/react-native";
import { getCommandLog } from "@/backoffice/logs/logsApi";
import { LogsScreen } from "@/backoffice/logs/LogsScreen";
import { translate } from "@/i18n/translate";
import { renderBackofficeScreen } from "@/testing/renderBackofficeScreen";

jest.mock("@/backoffice/logs/logsApi", () => ({
  ...jest.requireActual("@/backoffice/logs/logsApi"),
  getCommandLog: jest.fn(),
}));

describe("When the audit log is not connected", () => {
  it("Then a warning replaces the empty day", async () => {
    jest.mocked(getCommandLog).mockResolvedValue({
      day: "2026-10-21",
      entries: [],
      isTruncated: false,
      isConnected: false,
    });

    await renderBackofficeScreen(<LogsScreen />);

    expect(await screen.findByText(translate("backoffice.logs.notConnected"))).toBeOnTheScreen();
    expect(screen.queryByText(translate("backoffice.logs.noCommands"))).not.toBeOnTheScreen();
  });
});
