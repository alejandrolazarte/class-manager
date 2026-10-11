import { screen } from "@testing-library/react-native";
import { AppearanceSettings } from "@/features/settings/components/AppearanceSettings";
import { translate } from "@/i18n/translate";
import { renderWithProviders } from "@/testing/renderWithProviders";

describe("When appearance is shown", () => {
  it("Then no theme can be chosen", async () => {
    await renderWithProviders(
      <AppearanceSettings colorsHint={translate("settings.colorsHint.student")} />,
    );

    expect(screen.queryByRole("button", { name: translate("themes.violet") })).toBeNull();
    expect(screen.getByText(translate("settings.colorsHint.student"))).toBeOnTheScreen();
  });
});
