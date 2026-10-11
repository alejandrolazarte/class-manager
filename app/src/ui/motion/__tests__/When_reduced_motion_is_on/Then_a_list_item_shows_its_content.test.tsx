import { render, screen } from "@testing-library/react-native";
import { ThemeProvider } from "@/theme/ThemeProvider";
import { AppText } from "@/ui/AppText";
import { AnimatedListItem } from "@/ui/motion/AnimatedListItem";

jest.mock("@/ui/motion/useReducedMotion", () => ({ useReducedMotion: () => true }));

describe("When reduced motion is on", () => {
  it("Then a list item shows its content", async () => {
    await render(
      <ThemeProvider initialColorSchemePreference="light">
        <AnimatedListItem index={0}>
          <AppText>Fila</AppText>
        </AnimatedListItem>
      </ThemeProvider>,
    );

    expect(screen.getByText("Fila")).toBeTruthy();
  });
});
