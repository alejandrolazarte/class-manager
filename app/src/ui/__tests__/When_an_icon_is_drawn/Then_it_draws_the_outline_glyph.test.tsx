import Ionicons from "@expo/vector-icons/Ionicons";
import { render, screen } from "@testing-library/react-native";
import { ThemeProvider } from "@/theme/ThemeProvider";
import { Icon } from "@/ui/Icon";

describe("When an icon is drawn", () => {
  it("Then it draws the outline glyph", async () => {
    await render(
      <ThemeProvider initialColorSchemePreference="light">
        <Icon name="home" accessibilityLabel="Inicio" />
      </ThemeProvider>,
    );

    const outlineGlyph = String.fromCodePoint(Ionicons.glyphMap["home-outline"] as number);
    expect(screen.getByLabelText("Inicio")).toHaveTextContent(outlineGlyph);
  });
});
