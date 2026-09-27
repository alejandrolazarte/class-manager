import { render, screen } from "@testing-library/react-native";
import { ThemeProvider } from "@/theme/ThemeProvider";
import { Icon } from "@/ui/Icon";

describe("When icon has no label", () => {
  it("Then it is hidden from accessibility", async () => {
    await render(
      <ThemeProvider initialColorSchemePreference="light">
        <Icon name="add" testID="decorative-icon" />
      </ThemeProvider>,
    );

    expect(screen.getByTestId("decorative-icon", { includeHiddenElements: true })).toBeTruthy();
    expect(screen.queryByTestId("decorative-icon")).toBeNull();
  });
});
