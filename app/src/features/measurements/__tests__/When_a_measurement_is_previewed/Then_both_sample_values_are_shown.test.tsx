import { fireEvent, screen } from "@testing-library/react-native";
import { NewMeasurementScreen } from "@/features/measurements/screens/NewMeasurementScreen";
import { translate } from "@/i18n/translate";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/measurements/measurementsApi");

describe("When a measurement is previewed", () => {
  it("Then both sample values are shown", async () => {
    await renderWithProviders(<NewMeasurementScreen />);

    await fireEvent.press(
      screen.getByRole("button", { name: translate("measurements.kind.time") }),
    );
    await fireEvent.press(
      screen.getByRole("button", { name: translate("measurements.improvesWhen.decreases") }),
    );

    expect(screen.getByText(translate("measurements.form.previewTitle"))).toBeOnTheScreen();
    expect(
      screen.getByText(
        translate("measurements.form.previewRange", { from: "42,50 s", to: "38,10 s" }),
      ),
    ).toBeOnTheScreen();
  });
});
