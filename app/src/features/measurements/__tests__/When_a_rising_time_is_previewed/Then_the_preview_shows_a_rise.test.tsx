import { fireEvent, screen } from "@testing-library/react-native";
import { NewMeasurementScreen } from "@/features/measurements/screens/NewMeasurementScreen";
import { translate } from "@/i18n/translate";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/measurements/measurementsApi");

describe("When a rising time is previewed", () => {
  it("Then the preview shows a rise", async () => {
    await renderWithProviders(<NewMeasurementScreen />);

    await fireEvent.press(
      screen.getByRole("button", { name: translate("measurements.kind.time") }),
    );
    await fireEvent.press(
      screen.getByRole("button", { name: translate("measurements.improvesWhen.increases") }),
    );

    expect(screen.getByText("+4,40 s")).toBeOnTheScreen();
  });
});
