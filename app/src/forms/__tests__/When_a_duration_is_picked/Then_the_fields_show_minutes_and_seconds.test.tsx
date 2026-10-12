import { fireEvent, screen, within } from "@testing-library/react-native";
import { translate } from "@/i18n/translate";
import {
  DurationFieldsHarness,
  harnessMinutesLabel,
  harnessSecondsLabel,
} from "@/testing/FieldHarness";
import { renderWithProviders } from "@/testing/renderWithProviders";

describe("When a duration is picked", () => {
  it("Then the fields show minutes and seconds", async () => {
    await renderWithProviders(<DurationFieldsHarness />);

    await fireEvent.press(screen.getByRole("button", { name: harnessSecondsLabel }));
    await fireEvent.press(
      screen.getByRole("button", { name: `${translate("durationField.minutes")} 04` }),
    );
    await fireEvent.press(
      screen.getByRole("button", { name: `${translate("durationField.seconds")} 32` }),
    );
    await fireEvent.press(screen.getByRole("button", { name: translate("dateField.done") }));

    expect(
      within(screen.getByRole("button", { name: harnessMinutesLabel })).getByText("04"),
    ).toBeOnTheScreen();
    expect(
      within(screen.getByRole("button", { name: harnessSecondsLabel })).getByText("32"),
    ).toBeOnTheScreen();
  });
});
