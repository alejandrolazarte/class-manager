import { render, screen } from "@testing-library/react-native";
import { Text } from "react-native";
import { PersistedThemePreferences } from "@/theme/PersistedThemePreferences";
import { ThemeProvider } from "@/theme/ThemeProvider";
import { themePreferenceStorage } from "@/theme/themePreferenceStorage";
import { serializeThemePreferences } from "@/theme/themePreferences";
import { themes } from "@/theme/themes";
import { useTheme } from "@/theme/useTheme";

jest.mock("@/theme/themePreferenceStorage", () => ({
  themePreferenceStorage: { read: jest.fn(), write: jest.fn() },
}));

function BackgroundColorProbe() {
  const { colors } = useTheme();
  return <Text>{colors.background}</Text>;
}

describe("When theme preferences were stored", () => {
  it("Then they are applied on start", async () => {
    jest
      .mocked(themePreferenceStorage.read)
      .mockResolvedValue(
        serializeThemePreferences({ themeName: "violet", colorSchemePreference: "dark" }),
      );
    jest.mocked(themePreferenceStorage.write).mockResolvedValue();

    await render(
      <ThemeProvider initialColorSchemePreference="light">
        <PersistedThemePreferences />
        <BackgroundColorProbe />
      </ThemeProvider>,
    );

    expect(await screen.findByText(themes.violet.dark.background)).toBeTruthy();
  });
});
