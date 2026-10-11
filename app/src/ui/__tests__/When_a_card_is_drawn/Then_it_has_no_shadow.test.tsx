import { render, screen } from "@testing-library/react-native";
import { StyleSheet } from "react-native";
import { ThemeProvider } from "@/theme/ThemeProvider";
import { AppText } from "@/ui/AppText";
import { Card } from "@/ui/Card";

describe("When a card is drawn", () => {
  it("Then it has no shadow", async () => {
    await render(
      <ThemeProvider initialColorSchemePreference="light">
        <Card testID="flat-card">
          <AppText>Contenido</AppText>
        </Card>
      </ThemeProvider>,
    );

    const cardStyle = StyleSheet.flatten(screen.getByTestId("flat-card").props.style) ?? {};
    expect(cardStyle).not.toHaveProperty("shadowOpacity");
    expect(cardStyle).not.toHaveProperty("elevation");
  });
});
