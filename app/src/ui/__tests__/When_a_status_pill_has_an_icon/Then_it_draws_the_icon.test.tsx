import Ionicons from "@expo/vector-icons/Ionicons";
import { render, screen } from "@testing-library/react-native";
import { ThemeProvider } from "@/theme/ThemeProvider";
import { StatusPill } from "@/ui/StatusPill";

describe("When a status pill has an icon", () => {
  it("Then it draws the icon", async () => {
    await render(
      <ThemeProvider initialColorSchemePreference="light">
        <StatusPill label="Evento" tone="success" icon="celebration" />
      </ThemeProvider>,
    );

    const glyph = String.fromCodePoint(Ionicons.glyphMap["sparkles-outline"] as number);
    expect(screen.getByText(glyph, { includeHiddenElements: true })).toBeOnTheScreen();
  });
});
