import { render, screen } from "@testing-library/react-native";
import { ThemeProvider } from "@/theme/ThemeProvider";
import { MedalBadge } from "@/ui/MedalBadge";

describe("When a medal badge is drawn", () => {
  it("Then it is hidden from screen readers", async () => {
    await render(
      <ThemeProvider initialColorSchemePreference="light">
        <MedalBadge icon="flag" metal="gold" />
      </ThemeProvider>,
    );

    expect(screen.queryByRole("image")).toBeNull();
    expect(screen.getByTestId("medal-badge", { includeHiddenElements: true })).toBeTruthy();
  });
});
