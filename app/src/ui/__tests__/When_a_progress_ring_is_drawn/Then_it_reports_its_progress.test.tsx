import { render, screen } from "@testing-library/react-native";
import { ThemeProvider } from "@/theme/ThemeProvider";
import { AppText } from "@/ui/AppText";
import { ProgressRing } from "@/ui/ProgressRing";

describe("When a progress ring is drawn", () => {
  it("Then it reports its progress", async () => {
    await render(
      <ThemeProvider initialColorSchemePreference="light">
        <ProgressRing ratio={0.4} accessibilityLabel="Nivel 2">
          <AppText>2</AppText>
        </ProgressRing>
      </ThemeProvider>,
    );

    expect(screen.getByRole("progressbar", { name: "Nivel 2" })).toHaveAccessibilityValue({
      now: 40,
    });
  });
});
