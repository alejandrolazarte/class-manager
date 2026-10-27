import { render, screen } from "@testing-library/react-native";
import { ThemeProvider } from "@/theme/ThemeProvider";
import { Confetti } from "@/ui/motion/Confetti";

jest.mock("@/ui/motion/useReducedMotion", () => ({ useReducedMotion: () => true }));

describe("When reduced motion is on", () => {
  it("Then no confetti falls", async () => {
    await render(
      <ThemeProvider initialColorSchemePreference="light">
        <Confetti />
      </ThemeProvider>,
    );

    expect(screen.queryByTestId("confetti", { includeHiddenElements: true })).toBeNull();
  });
});
