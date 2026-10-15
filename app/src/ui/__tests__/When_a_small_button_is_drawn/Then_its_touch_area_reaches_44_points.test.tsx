import { render, screen } from "@testing-library/react-native";
import { ThemeProvider } from "@/theme/ThemeProvider";
import { Button } from "@/ui/Button";

const smallButtonHeight = 32;
const minimumTouchSize = 44;

describe("When a small button is drawn", () => {
  it("Then its touch area reaches 44 points", async () => {
    await render(
      <ThemeProvider initialColorSchemePreference="light">
        <Button variant="ghost" size="small" label="Cobrar" onPress={jest.fn()} />
      </ThemeProvider>,
    );

    const { hitSlop } = screen.getByRole("button", { name: "Cobrar" }).props;

    expect(smallButtonHeight + hitSlop.top + hitSlop.bottom).toBeGreaterThanOrEqual(
      minimumTouchSize,
    );
  });
});
