import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { createMeasurement } from "@/features/measurements/measurementsApi";
import { NewMeasurementScreen } from "@/features/measurements/screens/NewMeasurementScreen";
import { translate } from "@/i18n/translate";
import { buildDistanceMeasurement } from "@/testing/measurementFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/measurements/measurementsApi");

describe("When owner creates a measurement", () => {
  beforeEach(() => {
    jest.mocked(createMeasurement).mockResolvedValue(buildDistanceMeasurement());
  });

  it("Then it is sent", async () => {
    await renderWithProviders(<NewMeasurementScreen />);

    await fireEvent.changeText(
      screen.getByLabelText(translate("measurements.form.name")),
      "Nado continuo",
    );
    await fireEvent.press(
      screen.getByRole("button", { name: translate("measurements.kind.distance") }),
    );
    await fireEvent.press(
      screen.getByRole("button", { name: translate("measurements.improvesWhen.increases") }),
    );
    await fireEvent.press(
      screen.getByRole("button", { name: translate("measurements.form.recorderStudents") }),
    );
    await fireEvent.press(
      screen.getByRole("button", { name: translate("measurements.form.create") }),
    );

    await waitFor(() =>
      expect(createMeasurement).toHaveBeenCalledWith({
        name: "Nado continuo",
        kind: "Distance",
        unit: "m",
        improvesWhen: "Increases",
        isVisibleToStudents: true,
        isRecordedByStudents: true,
      }),
    );
  });
});
