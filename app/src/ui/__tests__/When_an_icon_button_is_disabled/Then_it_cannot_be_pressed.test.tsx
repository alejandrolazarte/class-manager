import { fireEvent, render, screen } from "@testing-library/react-native";
import { ThemeProvider } from "@/theme/ThemeProvider";
import { IconButton } from "@/ui/IconButton";

describe("When an icon button is disabled", () => {
  it("Then it cannot be pressed", async () => {
    const goToNextMonth = jest.fn();
    await render(
      <ThemeProvider initialColorSchemePreference="light">
        <IconButton
          icon="next"
          accessibilityLabel="Mes siguiente"
          onPress={goToNextMonth}
          disabled
        />
      </ThemeProvider>,
    );

    await fireEvent.press(screen.getByRole("button", { name: "Mes siguiente" }));

    expect(goToNextMonth).not.toHaveBeenCalled();
  });
});
