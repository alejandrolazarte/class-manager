import { fireEvent, render, screen } from "@testing-library/react-native";
import { ThemeProvider } from "@/theme/ThemeProvider";
import { FloatingBarButton } from "@/ui/FloatingBarButton";

describe("When a floating bar button is pressed", () => {
  it("Then it runs its action", async () => {
    const openCart = jest.fn();
    await render(
      <ThemeProvider initialColorSchemePreference="light">
        <FloatingBarButton
          label="Ver carrito"
          count={3}
          value="$40.000"
          accessibilityLabel="Ver carrito, 3, $40.000"
          onPress={openCart}
        />
      </ThemeProvider>,
    );

    await fireEvent.press(screen.getByRole("button", { name: "Ver carrito, 3, $40.000" }));

    expect(openCart).toHaveBeenCalledTimes(1);
  });
});
