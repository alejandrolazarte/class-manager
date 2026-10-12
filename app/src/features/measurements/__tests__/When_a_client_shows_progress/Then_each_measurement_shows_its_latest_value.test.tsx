import { fireEvent, screen } from "@testing-library/react-native";
import { getClient } from "@/features/clients/clientsApi";
import { ClientDetailScreen } from "@/features/clients/screens/ClientDetailScreen";
import { getStudentMeasurements } from "@/features/measurements/measurementsApi";
import { translate } from "@/i18n/translate";
import { buildClient } from "@/testing/clientFactory";
import {
  buildMeasurementRecord,
  buildStudentMeasurement,
  buildTimeMeasurement,
} from "@/testing/measurementFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";
import { buildStudent } from "@/testing/studentFactory";

jest.mock("@/features/clients/clientsApi");
jest.mock("@/features/measurements/measurementsApi");
jest.mock("@/features/enrollments/enrollmentsApi");
jest.mock("@/features/fees/feesApi");

describe("When a client shows progress", () => {
  beforeEach(() => {
    jest.mocked(getClient).mockResolvedValue(buildClient({ students: [buildStudent()] }));
    jest
      .mocked(getStudentMeasurements)
      .mockResolvedValue([
        buildStudentMeasurement(buildTimeMeasurement(), [
          buildMeasurementRecord("2026-04-07", 38.4),
          buildMeasurementRecord("2026-10-06", 29.7),
        ]),
      ]);
  });

  it("Then each measurement shows its latest value", async () => {
    await renderWithProviders(<ClientDetailScreen clientId="client-1" />);

    await fireEvent.press(
      await screen.findByRole("tab", { name: translate("clients.detail.progress") }),
    );

    expect(await screen.findByText("29,70 s")).toBeOnTheScreen();
    expect(screen.getByText("−8,70 s")).toBeOnTheScreen();
  });
});
