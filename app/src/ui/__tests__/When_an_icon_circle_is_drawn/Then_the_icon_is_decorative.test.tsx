import { render, screen } from "@testing-library/react-native";
import { ThemeProvider } from "@/theme/ThemeProvider";
import { IconCircle } from "@/ui/IconCircle";

describe("When an icon circle is drawn", () => {
  it("Then the icon is decorative", async () => {
    await render(
      <ThemeProvider initialColorSchemePreference="light">
        <IconCircle icon="students" />
      </ThemeProvider>,
    );

    expect(screen.queryByRole("image")).toBeNull();
  });
});
