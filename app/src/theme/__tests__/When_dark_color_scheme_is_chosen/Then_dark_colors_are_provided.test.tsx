import { render, screen } from "@testing-library/react-native";
import { Text } from "react-native";
import { ThemeProvider } from "@/theme/ThemeProvider";
import { themes } from "@/theme/themes";
import { useTheme } from "@/theme/useTheme";

function BackgroundColorProbe() {
  const { colors } = useTheme();
  return <Text>{colors.background}</Text>;
}

describe("When dark color scheme is chosen", () => {
  it("Then dark colors are provided", async () => {
    await render(
      <ThemeProvider initialThemeName="violet" initialColorSchemePreference="dark">
        <BackgroundColorProbe />
      </ThemeProvider>,
    );

    expect(screen.getByText(themes.violet.dark.background)).toBeTruthy();
  });
});
