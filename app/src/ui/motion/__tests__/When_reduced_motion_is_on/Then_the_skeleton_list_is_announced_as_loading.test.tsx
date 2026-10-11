import { render, screen } from "@testing-library/react-native";
import { ThemeProvider } from "@/theme/ThemeProvider";
import { SkeletonList } from "@/ui/motion/SkeletonList";

jest.mock("@/ui/motion/useReducedMotion", () => ({ useReducedMotion: () => true }));

describe("When reduced motion is on", () => {
  it("Then the skeleton list is announced as loading", async () => {
    await render(
      <ThemeProvider initialColorSchemePreference="light">
        <SkeletonList />
      </ThemeProvider>,
    );

    expect(screen.getByRole("progressbar", { name: "Cargando…" })).toBeTruthy();
  });
});
