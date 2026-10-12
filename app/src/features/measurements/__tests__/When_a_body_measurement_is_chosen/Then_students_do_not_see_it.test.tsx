import { fireEvent, screen } from "@testing-library/react-native";
import { NewMeasurementScreen } from "@/features/measurements/screens/NewMeasurementScreen";
import { translate } from "@/i18n/translate";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/measurements/measurementsApi");

describe("When a body measurement is chosen", () => {
  it("Then students do not see it", async () => {
    await renderWithProviders(<NewMeasurementScreen />);

    await fireEvent.press(
      screen.getByRole("button", { name: translate("measurements.kind.weight") }),
    );

    expect(
      screen.getByRole("switch", { name: translate("measurements.visibleToStudents") }),
    ).not.toBeChecked();
  });
});
