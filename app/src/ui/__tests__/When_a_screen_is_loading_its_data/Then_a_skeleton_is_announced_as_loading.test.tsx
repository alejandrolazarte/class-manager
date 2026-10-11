import { render, screen } from "@testing-library/react-native";
import { ThemeProvider } from "@/theme/ThemeProvider";
import { SkeletonScreen } from "@/ui/SkeletonScreen";

describe("When a screen is loading its data", () => {
  it("Then a skeleton is announced as loading", async () => {
    await render(
      <ThemeProvider initialColorSchemePreference="light">
        <SkeletonScreen />
      </ThemeProvider>,
    );

    expect(screen.getByRole("progressbar", { name: "Cargando…" })).toBeOnTheScreen();
  });
});
