import { render, screen } from "@testing-library/react-native";
import { StyleSheet } from "react-native";
import { ThemeProvider } from "@/theme/ThemeProvider";
import { AppText } from "@/ui/AppText";
import { TonalBlock } from "@/ui/TonalBlock";

describe("When a tonal block is drawn", () => {
  it("Then it shows its content", async () => {
    await render(
      <ThemeProvider initialColorSchemePreference="light">
        <TonalBlock tone="primary" testID="next-class">
          <AppText>Próxima clase</AppText>
        </TonalBlock>
      </ThemeProvider>,
    );

    expect(screen.getByText("Próxima clase")).toBeOnTheScreen();
    const highlightStyle = StyleSheet.flatten(screen.getByTestId("next-class").props.style) ?? {};
    expect(highlightStyle).not.toHaveProperty("shadowOpacity");
  });
});
