import { fireEvent, screen } from "@testing-library/react-native";
import { AppearanceSettings } from "@/features/settings/components/ThemePicker";
import { translate } from "@/i18n/translate";
import { renderWithProviders } from "@/testing/renderWithProviders";
import { themes } from "@/theme/themes";
import { useTheme } from "@/theme/useTheme";
import { AppText } from "@/ui/AppText";

function BackgroundColorProbe() {
  const { colors } = useTheme();
  return <AppText>{colors.background}</AppText>;
}

describe("When dark appearance is chosen", () => {
  it("Then dark colors are used", async () => {
    await renderWithProviders(
      <>
        <AppearanceSettings />
        <BackgroundColorProbe />
      </>,
    );

    await fireEvent.press(
      screen.getByRole("button", { name: translate("settings.appearance.dark") }),
    );

    expect(screen.getByText(themes.aqua.dark.background)).toBeOnTheScreen();
  });
});
