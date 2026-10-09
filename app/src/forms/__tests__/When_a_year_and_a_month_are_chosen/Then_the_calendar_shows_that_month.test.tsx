import { fireEvent, screen } from "@testing-library/react-native";
import { translate } from "@/i18n/translate";
import { BirthDateFieldHarness } from "@/testing/FieldHarness";
import { renderWithProviders } from "@/testing/renderWithProviders";

describe("When a year and a month are chosen", () => {
  it("Then the calendar shows that month", async () => {
    await renderWithProviders(<BirthDateFieldHarness />);

    await fireEvent.press(
      screen.getByRole("button", { name: translate("dateField.openCalendar") }),
    );
    await fireEvent.press(screen.getByRole("button", { name: translate("dateField.chooseYear") }));
    await fireEvent.press(screen.getByRole("button", { name: "1985" }));
    await fireEvent.press(screen.getByRole("button", { name: "Junio 1985" }));

    expect(screen.getByRole("button", { name: "20 junio 1985" })).toBeTruthy();
  });
});
