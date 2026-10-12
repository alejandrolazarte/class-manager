import { screen } from "@testing-library/react-native";
import { NewMeasurementScreen } from "@/features/measurements/screens/NewMeasurementScreen";
import { translate } from "@/i18n/translate";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/measurements/measurementsApi");

describe("When new measurement form is empty", () => {
  it("Then create stays disabled", async () => {
    await renderWithProviders(<NewMeasurementScreen />);

    expect(
      screen.getByRole("button", { name: translate("measurements.form.create") }),
    ).toBeDisabled();
  });
});
