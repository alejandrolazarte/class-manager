import { fireEvent, screen } from "@testing-library/react-native";
import { NewMeasurementScreen } from "@/features/measurements/screens/NewMeasurementScreen";
import { translate } from "@/i18n/translate";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/measurements/measurementsApi");

describe("When a falling time is previewed", () => {
  it("Then the preview shows a drop", async () => {
    await renderWithProviders(<NewMeasurementScreen />);

    await fireEvent.press(
      screen.getByRole("button", { name: translate("measurements.kind.time") }),
    );
    await fireEvent.press(
      screen.getByRole("button", { name: translate("measurements.improvesWhen.decreases") }),
    );

    expect(screen.getByText("−4,40 s")).toBeOnTheScreen();
  });
});
