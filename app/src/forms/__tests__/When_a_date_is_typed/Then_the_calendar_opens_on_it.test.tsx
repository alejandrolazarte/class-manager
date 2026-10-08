import { fireEvent, screen } from "@testing-library/react-native";
import { translate } from "@/i18n/translate";
import { DateFieldHarness, harnessDateLabel } from "@/testing/FieldHarness";
import { renderWithProviders } from "@/testing/renderWithProviders";

describe("When a date is typed", () => {
  it("Then the calendar opens on it", async () => {
    await renderWithProviders(<DateFieldHarness />);

    await fireEvent.changeText(screen.getByLabelText(harnessDateLabel), "21072024");
    await fireEvent.press(
      screen.getByRole("button", {
        name: translate("dateField.openCalendar"),
      }),
    );

    expect(screen.getByRole("button", { name: "21 julio 2024", selected: true })).toBeTruthy();
  });
});
