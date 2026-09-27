import { fireEvent, render, screen } from "@testing-library/react-native";
import { Pressable, Text } from "react-native";
import { ThemeProvider } from "@/theme/ThemeProvider";
import { themes } from "@/theme/themes";
import { useTheme } from "@/theme/useTheme";

const switchToOceanLabel = "switch to ocean";

function PrimaryColorProbe() {
  const { colors, setThemeName } = useTheme();
  return (
    <>
      <Text>{colors.primary}</Text>
      <Pressable
        accessibilityRole="button"
        accessibilityLabel={switchToOceanLabel}
        onPress={() => setThemeName("ocean")}
      />
    </>
  );
}

describe("When theme is changed", () => {
  it("Then new theme colors are provided", async () => {
    await render(
      <ThemeProvider initialThemeName="violet" initialColorSchemePreference="light">
        <PrimaryColorProbe />
      </ThemeProvider>,
    );

    await fireEvent.press(screen.getByLabelText(switchToOceanLabel));

    expect(screen.getByText(themes.ocean.light.primary)).toBeTruthy();
  });
});
