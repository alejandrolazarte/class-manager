import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { listMeasurements, updateMeasurement } from "@/features/measurements/measurementsApi";
import { MeasurementSettingsScreen } from "@/features/measurements/screens/MeasurementSettingsScreen";
import { translate } from "@/i18n/translate";
import { buildDistanceMeasurement } from "@/testing/measurementFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/measurements/measurementsApi");

const measurement = buildDistanceMeasurement();

describe("When a measurement is hidden from students", () => {
  beforeEach(() => {
    jest.mocked(listMeasurements).mockResolvedValue([measurement]);
    jest.mocked(updateMeasurement).mockResolvedValue(measurement);
  });

  it("Then students stop recording it", async () => {
    await renderWithProviders(<MeasurementSettingsScreen />);

    await fireEvent.press(
      await screen.findByRole("switch", {
        name: translate("measurements.visibleToStudentsAccessibility", { name: measurement.name }),
      }),
    );

    await waitFor(() =>
      expect(updateMeasurement).toHaveBeenCalledWith(measurement.id, {
        name: measurement.name,
        improvesWhen: measurement.improvesWhen,
        isVisibleToStudents: false,
        isRecordedByStudents: false,
      }),
    );
  });
});
