import { fireEvent, screen } from "@testing-library/react-native";
import { translate } from "@/i18n/translate";
import { harnessTimeLabel, TimeFieldHarness } from "@/testing/FieldHarness";
import { renderWithProviders } from "@/testing/renderWithProviders";

describe("When a time is typed", () => {
  it("Then the picker keeps its minutes", async () => {
    await renderWithProviders(<TimeFieldHarness />);

    await fireEvent.changeText(screen.getByLabelText(harnessTimeLabel), "1847");
    await fireEvent.press(
      screen.getByRole("button", {
        name: translate("timeField.openPicker"),
      }),
    );

    expect(
      screen.getByRole("button", { name: `${translate("timeField.minutes")} 47`, selected: true }),
    ).toBeTruthy();
  });
});
