import { render, screen } from "@testing-library/react-native";
import { StyleSheet } from "react-native";
import { ThemeProvider } from "@/theme/ThemeProvider";
import { AppText } from "@/ui/AppText";
import { Highlight } from "@/ui/Highlight";

describe("When a highlight is drawn", () => {
  it("Then it shows its content without a shadow", async () => {
    await render(
      <ThemeProvider initialColorSchemePreference="light">
        <Highlight tone="primary" testID="next-class">
          <AppText>Próxima clase</AppText>
        </Highlight>
      </ThemeProvider>,
    );

    expect(screen.getByText("Próxima clase")).toBeOnTheScreen();
    const highlightStyle = StyleSheet.flatten(screen.getByTestId("next-class").props.style) ?? {};
    expect(highlightStyle).not.toHaveProperty("shadowOpacity");
  });
});
