import { render, screen } from "@testing-library/react-native";
import { TabLabel } from "@/ui/TabBar";
import { ThemeProvider } from "@/theme/ThemeProvider";

describe("When a tab is focused", () => {
  it("Then the active mark sits under its label", async () => {
    await render(
      <ThemeProvider initialColorSchemePreference="light">
        <TabLabel label="Hoy" isFocused />
      </ThemeProvider>,
    );

    const renderedTree = JSON.stringify(screen.toJSON());
    expect(renderedTree.indexOf("tab-active-mark")).toBeGreaterThan(renderedTree.indexOf("Hoy"));
  });
});
