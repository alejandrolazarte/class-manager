import { render, screen } from "@testing-library/react-native";
import * as ReactNative from "react-native";
import { ThemeProvider } from "@/theme/ThemeProvider";
import { themes } from "@/theme/themes";
import { useTheme } from "@/theme/useTheme";

function ColorSchemeProbe() {
  const { colorScheme, colors } = useTheme();
  return <ReactNative.Text>{`${colorScheme} ${colors.background}`}</ReactNative.Text>;
}

describe("When system color scheme is dark", () => {
  beforeEach(() => {
    jest.spyOn(ReactNative, "useColorScheme").mockReturnValue("dark");
  });

  afterEach(() => {
    jest.restoreAllMocks();
  });

  it("Then dark colors are provided", async () => {
    await render(
      <ThemeProvider initialColorSchemePreference="system">
        <ColorSchemeProbe />
      </ThemeProvider>,
    );

    expect(screen.getByText(`dark ${themes.violet.dark.background}`)).toBeTruthy();
  });
});
