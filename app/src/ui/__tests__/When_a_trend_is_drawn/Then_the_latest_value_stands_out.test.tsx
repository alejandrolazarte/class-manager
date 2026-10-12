import { render, screen } from "@testing-library/react-native";
import { StyleSheet } from "react-native";
import { ThemeProvider } from "@/theme/ThemeProvider";
import { TrendBars } from "@/ui/TrendBars";

describe("When a trend is drawn", () => {
  it("Then the latest value stands out", async () => {
    await render(
      <ThemeProvider initialColorSchemePreference="light">
        <TrendBars values={[38.4, 33.1, 29.7]} />
      </ThemeProvider>,
    );

    expect(screen.getAllByTestId("trend-bar")).toHaveLength(2);
    expect(StyleSheet.flatten(screen.getByTestId("trend-bar-latest").props.style).height).toBe(
      "15%",
    );
  });
});
