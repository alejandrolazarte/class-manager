import { screen } from "@testing-library/react-native";
import { getCommandLog } from "@/backoffice/logs/logsApi";
import { LogsScreen } from "@/backoffice/logs/LogsScreen";
import { aCommandLogEntry } from "@/testing/backofficeLogs";
import { renderBackofficeScreen } from "@/testing/renderBackofficeScreen";

jest.mock("@/backoffice/logs/logsApi", () => ({
  ...jest.requireActual("@/backoffice/logs/logsApi"),
  getCommandLog: jest.fn(),
}));

describe("When the logs open", () => {
  it("Then each action of the day shows its brand and person", async () => {
    jest
      .mocked(getCommandLog)
      .mockResolvedValue({ day: "2026-10-21", entries: [aCommandLogEntry()], isTruncated: false });

    await renderBackofficeScreen(<LogsScreen />);

    expect(await screen.findByText("RecordPayment")).toBeOnTheScreen();
    expect(screen.getByText("14:32 · Estudio Centro · Sede Norte · Ana Pérez")).toBeOnTheScreen();
  });
});
