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

describe("When the operator opens an action", () => {
  it("Then its details and data are shown", async () => {
    jest.mocked(getCommandLog).mockResolvedValue({
      day: "2026-10-21",
      entries: [
        aCommandLogEntry({
          outcome: "Failed",
          errorKind: "Validation",
          errorCode: "payment.amount_required",
        }),
      ],
      isTruncated: false,
      isConnected: true,
    });
    await renderBackofficeScreen(<LogsScreen />);

    await fireEvent.press(await screen.findByRole("button", { name: "RecordPayment" }));

    expect(screen.getByText("Validation · payment.amount_required")).toBeOnTheScreen();
    expect(
      screen.getByText(translate("backoffice.logs.milliseconds", { count: 18 })),
    ).toBeOnTheScreen();
    expect(screen.getByText(/"Amount": 25000/)).toBeOnTheScreen();
  });
});
