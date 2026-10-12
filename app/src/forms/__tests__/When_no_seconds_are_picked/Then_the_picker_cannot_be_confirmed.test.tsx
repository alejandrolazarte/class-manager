import { fireEvent, screen } from "@testing-library/react-native";
import { translate } from "@/i18n/translate";
import { DurationFieldsHarness, harnessMinutesLabel } from "@/testing/FieldHarness";
import { renderWithProviders } from "@/testing/renderWithProviders";

describe("When no seconds are picked", () => {
  it("Then the picker cannot be confirmed", async () => {
    await renderWithProviders(<DurationFieldsHarness />);

    await fireEvent.press(screen.getByRole("button", { name: harnessMinutesLabel }));
    await fireEvent.press(
      screen.getByRole("button", { name: `${translate("durationField.minutes")} 04` }),
    );

    expect(screen.getByRole("button", { name: translate("dateField.done") })).toBeDisabled();
  });
});
