import { render, screen } from "@testing-library/react-native";
import { ThemeProvider } from "@/theme/ThemeProvider";
import { StreakFlame } from "@/ui/StreakFlame";

describe("When a streak flame is out", () => {
  it("Then the unlit flame is drawn", async () => {
    await render(
      <ThemeProvider initialColorSchemePreference="light">
        <StreakFlame isLit={false} />
      </ThemeProvider>,
    );

    expect(screen.getByTestId("streak-flame-out", { includeHiddenElements: true })).toBeTruthy();
  });
});
