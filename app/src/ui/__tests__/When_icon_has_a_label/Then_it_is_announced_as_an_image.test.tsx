import { render, screen } from "@testing-library/react-native";
import { ThemeProvider } from "@/theme/ThemeProvider";
import { Icon } from "@/ui/Icon";

const iconLabel = "Pagado";

describe("When icon has a label", () => {
  it("Then it is announced as an image", async () => {
    await render(
      <ThemeProvider initialColorSchemePreference="light">
        <Icon name="paid" accessibilityLabel={iconLabel} />
      </ThemeProvider>,
    );

    expect(screen.getByRole("image", { name: iconLabel })).toBeTruthy();
  });
});
