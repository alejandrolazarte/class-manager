import { render, screen } from "@testing-library/react-native";
import { TabIcon } from "@/navigation/tabBar";
import { ThemeProvider } from "@/theme/ThemeProvider";

describe("When a tab is focused", () => {
  it("Then its icon shows the active mark", async () => {
    await render(
      <ThemeProvider initialColorSchemePreference="light">
        <TabIcon icon="today" isFocused />
        <TabIcon icon="classes" isFocused={false} />
      </ThemeProvider>,
    );

    expect(screen.getAllByTestId("tab-active-mark")).toHaveLength(1);
  });
});
