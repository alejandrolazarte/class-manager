import { render, screen } from "@testing-library/react-native";
import { Text } from "react-native";
import { ThemeProvider } from "@/theme/ThemeProvider";
import { themes } from "@/theme/themes";
import { useTheme } from "@/theme/useTheme";

function PrimaryColorProbe() {
  const { colors } = useTheme();
  return <Text>{colors.primary}</Text>;
}

describe("When no theme is chosen", () => {
  it("Then the Laztty violet is used", async () => {
    await render(
      <ThemeProvider initialColorSchemePreference="light">
        <PrimaryColorProbe />
      </ThemeProvider>,
    );

    expect(screen.getByText(themes.violet.light.primary)).toBeTruthy();
  });
});
