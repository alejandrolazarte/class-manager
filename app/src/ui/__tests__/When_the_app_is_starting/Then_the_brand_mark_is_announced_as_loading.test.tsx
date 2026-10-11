import { render, screen, within } from "@testing-library/react-native";
import { ThemeProvider } from "@/theme/ThemeProvider";
import { LoadingScreen } from "@/ui/LoadingScreen";

describe("When the app is starting", () => {
  it("Then the brand mark is announced as loading", async () => {
    await render(
      <ThemeProvider initialColorSchemePreference="light">
        <LoadingScreen />
      </ThemeProvider>,
    );

    const progress = screen.getByRole("progressbar", { name: "Cargando…" });
    expect(within(progress).getByTestId("brand-mark")).toBeTruthy();
  });
});
