import { fireEvent, screen } from "@testing-library/react-native";
import { translate } from "@/i18n/translate";
import { harnessTimeLabel, TimeFieldHarness } from "@/testing/FieldHarness";
import { renderWithProviders } from "@/testing/renderWithProviders";

describe("When a time is picked", () => {
  it("Then the field shows it as hours and minutes", async () => {
    await renderWithProviders(<TimeFieldHarness />);

    await fireEvent.press(
      screen.getByRole("button", {
        name: translate("timeField.openPicker"),
      }),
    );
    await fireEvent.press(
      screen.getByRole("button", { name: `${translate("timeField.hours")} 08` }),
    );
    await fireEvent.press(
      screen.getByRole("button", { name: `${translate("timeField.minutes")} 30` }),
    );
    await fireEvent.press(screen.getByRole("button", { name: translate("dateField.done") }));

    expect(screen.getByLabelText(harnessTimeLabel).props.value).toBe("08:30");
  });
});
