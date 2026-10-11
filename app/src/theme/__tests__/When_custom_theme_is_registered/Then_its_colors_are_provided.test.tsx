import { render, screen } from "@testing-library/react-native";
import { Text } from "react-native";
import { ThemeProvider } from "@/theme/ThemeProvider";
import { ThemeDefinition, themes } from "@/theme/themes";
import { useTheme } from "@/theme/useTheme";

const customThemeName = "studio-brand";
const customPrimaryColor = "#b0306a";

const customTheme: ThemeDefinition = {
  light: { ...themes.aqua.light, primary: customPrimaryColor },
  dark: themes.aqua.dark,
};

function PrimaryColorProbe() {
  const { colors } = useTheme();
  return <Text>{colors.primary}</Text>;
}

describe("When custom theme is registered", () => {
  it("Then its colors are provided", async () => {
    await render(
      <ThemeProvider
        customThemes={{ [customThemeName]: customTheme }}
        baseThemeName={customThemeName}
        initialColorSchemePreference="light"
      >
        <PrimaryColorProbe />
      </ThemeProvider>,
    );

    expect(screen.getByText(customPrimaryColor)).toBeTruthy();
  });
});
