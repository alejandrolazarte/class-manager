import { render, screen } from "@testing-library/react-native";
import { ThemeProvider } from "@/theme/ThemeProvider";
import { DoodlePattern } from "@/ui/DoodlePattern";

describe("When a doodle pattern is drawn", () => {
  it("Then it is hidden from screen readers", async () => {
    await render(
      <ThemeProvider initialColorSchemePreference="light">
        <DoodlePattern />
      </ThemeProvider>,
    );

    expect(screen.queryByRole("image")).toBeNull();
    expect(screen.getByTestId("doodle-pattern", { includeHiddenElements: true })).toBeTruthy();
  });
});
