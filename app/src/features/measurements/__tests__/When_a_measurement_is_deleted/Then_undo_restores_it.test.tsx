import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import {
  deleteMeasurement,
  listMeasurements,
  restoreMeasurement,
} from "@/features/measurements/measurementsApi";
import { MeasurementSettingsScreen } from "@/features/measurements/screens/MeasurementSettingsScreen";
import { translate } from "@/i18n/translate";
import { buildTimeMeasurement } from "@/testing/measurementFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/measurements/measurementsApi");

const measurement = buildTimeMeasurement();

describe("When a measurement is deleted", () => {
  beforeEach(() => {
    jest.mocked(listMeasurements).mockResolvedValue([measurement]);
    jest.mocked(deleteMeasurement).mockResolvedValue(undefined);
    jest.mocked(restoreMeasurement).mockResolvedValue(undefined);
  });

  it("Then undo restores it", async () => {
    await renderWithProviders(<MeasurementSettingsScreen />);

    await fireEvent.press(
      await screen.findByRole("button", {
        name: translate("measurements.delete", { name: measurement.name }),
      }),
    );
    await fireEvent.press(await screen.findByRole("button", { name: translate("common.undo") }));

    await waitFor(() => expect(restoreMeasurement).toHaveBeenCalledWith(measurement.id));
  });
});
