import { screen } from "@testing-library/react-native";
import { getClient } from "@/features/clients/clientsApi";
import { getStudentMeasurements } from "@/features/measurements/measurementsApi";
import { StudentMeasurementScreen } from "@/features/measurements/screens/StudentMeasurementScreen";
import { translate } from "@/i18n/translate";
import { buildClient } from "@/testing/clientFactory";
import {
  buildMeasurementRecord,
  buildStudentMeasurement,
  buildTimeMeasurement,
} from "@/testing/measurementFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/clients/clientsApi");
jest.mock("@/features/measurements/measurementsApi");

const measurement = buildTimeMeasurement();

describe("When a lower time is recorded", () => {
  beforeEach(() => {
    jest.mocked(getClient).mockResolvedValue(buildClient());
    jest
      .mocked(getStudentMeasurements)
      .mockResolvedValue([
        buildStudentMeasurement(measurement, [
          buildMeasurementRecord("2026-04-07", 38.4),
          buildMeasurementRecord("2026-10-06", 29.7),
        ]),
      ]);
  });

  it("Then the detail marks a personal best", async () => {
    await renderWithProviders(
      <StudentMeasurementScreen
        clientId="client-1"
        studentId="student-1"
        measurementId={measurement.id}
      />,
    );

    expect(
      await screen.findByText(translate("measurements.detail.personalBest")),
    ).toBeOnTheScreen();
  });
});
