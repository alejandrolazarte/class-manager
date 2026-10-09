import { fireEvent, screen } from "@testing-library/react-native";
import { translate } from "@/i18n/translate";
import { BirthDateFieldHarness } from "@/testing/FieldHarness";
import { renderWithProviders } from "@/testing/renderWithProviders";

describe("When a birth date calendar is opened", () => {
  it("Then future days cannot be picked", async () => {
    jest.useFakeTimers({ now: new Date(2026, 9, 8), doNotFake: ["setTimeout", "setImmediate"] });
    await renderWithProviders(<BirthDateFieldHarness />);

    await fireEvent.press(
      screen.getByRole("button", {
        name: translate("dateField.openCalendar"),
      }),
    );

    expect(screen.getByRole("button", { name: "9 octubre 2026" })).toBeDisabled();
    jest.useRealTimers();
  });
});
