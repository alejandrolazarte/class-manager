import { render, screen } from "@testing-library/react-native";
import { TabLabel } from "@/ui/TabBar";
import { ThemeProvider } from "@/theme/ThemeProvider";

describe("When a tab is focused", () => {
  it("Then its label shows the active mark", async () => {
    await render(
      <ThemeProvider initialColorSchemePreference="light">
        <TabLabel label="Hoy" isFocused />
        <TabLabel label="Clases" isFocused={false} />
      </ThemeProvider>,
    );

    expect(screen.getAllByTestId("tab-active-mark")).toHaveLength(1);
  });
});
