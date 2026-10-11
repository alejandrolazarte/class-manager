import { render, screen } from "@testing-library/react-native";
import { ThemeProvider } from "@/theme/ThemeProvider";
import { SkeletonDetail } from "@/ui/motion/SkeletonDetail";

jest.mock("@/ui/motion/useReducedMotion", () => ({ useReducedMotion: () => true }));

describe("When reduced motion is on", () => {
  it("Then the skeleton detail is announced as loading", async () => {
    await render(
      <ThemeProvider initialColorSchemePreference="light">
        <SkeletonDetail />
      </ThemeProvider>,
    );

    expect(screen.getByRole("progressbar", { name: "Cargando…" })).toBeTruthy();
  });
});
