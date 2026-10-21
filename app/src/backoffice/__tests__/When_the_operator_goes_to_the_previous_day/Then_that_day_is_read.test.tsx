import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { todayUtcIsoDate } from "@/backoffice/logs/logLabels";
import { getCommandLog } from "@/backoffice/logs/logsApi";
import { LogsScreen } from "@/backoffice/logs/LogsScreen";
import { addDays } from "@/dates/dates";
import { translate } from "@/i18n/translate";
import { renderBackofficeScreen } from "@/testing/renderBackofficeScreen";

jest.mock("@/backoffice/logs/logsApi", () => ({
  ...jest.requireActual("@/backoffice/logs/logsApi"),
  getCommandLog: jest.fn(),
}));

describe("When the operator goes to the previous day", () => {
  it("Then that day is read", async () => {
    jest
      .mocked(getCommandLog)
      .mockImplementation(async (day) => ({ day, entries: [], isTruncated: false }));
    await renderBackofficeScreen(<LogsScreen />);
    await screen.findByText(translate("backoffice.logs.noCommands"));
    expect(
      screen.getByRole("button", { name: translate("backoffice.logs.nextDay") }),
    ).toBeDisabled();

    await fireEvent.press(
      screen.getByRole("button", { name: translate("backoffice.logs.previousDay") }),
    );

    await waitFor(() =>
      expect(getCommandLog).toHaveBeenLastCalledWith(addDays(todayUtcIsoDate(), -1)),
    );
  });
});
