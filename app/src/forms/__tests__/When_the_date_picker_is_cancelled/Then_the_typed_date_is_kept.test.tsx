import { fireEvent, screen } from "@testing-library/react-native";
import { translate } from "@/i18n/translate";
import { DateFieldHarness, harnessDateLabel } from "@/testing/FieldHarness";
import { renderWithProviders } from "@/testing/renderWithProviders";

describe("When the date picker is cancelled", () => {
  it("Then the typed date is kept", async () => {
    await renderWithProviders(<DateFieldHarness initialValue="10/03/2025" />);

    await fireEvent.press(
      screen.getByRole("button", {
        name: translate("dateField.openCalendar"),
      }),
    );
    await fireEvent.press(screen.getByRole("button", { name: "15 marzo 2025" }));
    await fireEvent.press(screen.getByRole("button", { name: translate("common.cancel") }));

    expect(screen.getByLabelText(harnessDateLabel).props.value).toBe("10/03/2025");
  });
});
