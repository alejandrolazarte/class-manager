import { render, screen } from "@testing-library/react-native";
import { StyleSheet } from "react-native";
import { ThemeProvider } from "@/theme/ThemeProvider";
import { TrendBars } from "@/ui/TrendBars";

describe("When a lower value is better", () => {
  it("Then the lowest value draws the tallest bar", async () => {
    await render(
      <ThemeProvider initialColorSchemePreference="light">
        <TrendBars values={[38.4, 33.1, 29.7]} isLowerBetter />
      </ThemeProvider>,
    );

    expect(StyleSheet.flatten(screen.getByTestId("trend-bar-latest").props.style).height).toBe(
      "100%",
    );
  });
});
