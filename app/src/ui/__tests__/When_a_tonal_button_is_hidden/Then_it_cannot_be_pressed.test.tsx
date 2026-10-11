import { fireEvent, render, screen } from "@testing-library/react-native";
import { ThemeProvider } from "@/theme/ThemeProvider";
import { TonalButton } from "@/ui/TonalButton";

describe("When a tonal button is hidden", () => {
  it("Then it cannot be pressed", async () => {
    const goToToday = jest.fn();
    await render(
      <ThemeProvider initialColorSchemePreference="light">
        <TonalButton label="Hoy" icon="today" onPress={goToToday} isHidden />
      </ThemeProvider>,
    );

    await fireEvent.press(screen.getByText("Hoy", { includeHiddenElements: true }));

    expect(goToToday).not.toHaveBeenCalled();
  });
});
