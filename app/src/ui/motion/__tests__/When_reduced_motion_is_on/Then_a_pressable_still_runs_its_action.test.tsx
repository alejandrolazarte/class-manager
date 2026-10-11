import { fireEvent, render, screen } from "@testing-library/react-native";
import { ThemeProvider } from "@/theme/ThemeProvider";
import { Button } from "@/ui/Button";

jest.mock("@/ui/motion/useReducedMotion", () => ({ useReducedMotion: () => true }));

describe("When reduced motion is on", () => {
  it("Then a pressable still runs its action", async () => {
    const onPress = jest.fn();
    await render(
      <ThemeProvider initialColorSchemePreference="light">
        <Button label="Guardar" onPress={onPress} />
      </ThemeProvider>,
    );

    await fireEvent.press(screen.getByRole("button", { name: "Guardar" }));

    expect(onPress).toHaveBeenCalledTimes(1);
  });
});
