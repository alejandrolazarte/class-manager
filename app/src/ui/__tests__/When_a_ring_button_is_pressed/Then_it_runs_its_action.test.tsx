import { fireEvent, render, screen } from "@testing-library/react-native";
import { View } from "react-native";
import { ThemeProvider } from "@/theme/ThemeProvider";
import { RingButton } from "@/ui/RingButton";

describe("When a ring button is pressed", () => {
  it("Then it runs its action", async () => {
    const openSwitcher = jest.fn();
    await render(
      <ThemeProvider initialColorSchemePreference="light">
        <RingButton accessibilityLabel="Cambiar de cuenta" onPress={openSwitcher}>
          <View />
        </RingButton>
      </ThemeProvider>,
    );

    await fireEvent.press(screen.getByRole("button", { name: "Cambiar de cuenta" }));

    expect(openSwitcher).toHaveBeenCalledTimes(1);
  });
});
