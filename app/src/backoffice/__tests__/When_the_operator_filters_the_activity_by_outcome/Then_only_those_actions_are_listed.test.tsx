import { fireEvent, screen } from "@testing-library/react-native";
import { getCommandLog } from "@/backoffice/logs/logsApi";
import { LogsScreen } from "@/backoffice/logs/LogsScreen";
import { translate } from "@/i18n/translate";
import { aCommandLogEntry } from "@/testing/backofficeLogs";
import { renderBackofficeScreen } from "@/testing/renderBackofficeScreen";

jest.mock("@/backoffice/logs/logsApi", () => ({
  ...jest.requireActual("@/backoffice/logs/logsApi"),
  getCommandLog: jest.fn(),
}));

describe("When the operator filters the activity by outcome", () => {
  it("Then only those actions are listed", async () => {
    jest.mocked(getCommandLog).mockResolvedValue({
      day: "2026-10-21",
      entries: [
        aCommandLogEntry(),
        aCommandLogEntry({ command: "AddStudentCommand", outcome: "Exception" }),
      ],
      isTruncated: false,
      isConnected: true,
    });
    await renderBackofficeScreen(<LogsScreen />);
    await screen.findByText("RecordPayment");

    await fireEvent.press(
      screen.getByRole("button", { name: translate("backoffice.logs.filters.outcome") }),
    );
    await fireEvent.press(screen.getByText(translate("backoffice.logs.outcome.Exception")));

    expect(screen.getByText("AddStudent")).toBeOnTheScreen();
    expect(screen.queryByText("RecordPayment")).not.toBeOnTheScreen();
  });
});
