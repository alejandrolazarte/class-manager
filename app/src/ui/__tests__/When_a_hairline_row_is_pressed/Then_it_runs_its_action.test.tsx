import { fireEvent, render, screen } from "@testing-library/react-native";
import { ThemeProvider } from "@/theme/ThemeProvider";
import { AppText } from "@/ui/AppText";
import { HairlineRow } from "@/ui/HairlineRow";

describe("When a hairline row is pressed", () => {
  it("Then it runs its action", async () => {
    const openMaterial = jest.fn();
    await render(
      <ThemeProvider initialColorSchemePreference="light">
        <HairlineRow
          accessibilityRole="link"
          accessibilityLabel="Abrir material"
          onPress={openMaterial}
        >
          <AppText>Nivel inicial</AppText>
        </HairlineRow>
      </ThemeProvider>,
    );

    await fireEvent.press(screen.getByRole("link", { name: "Abrir material" }));

    expect(openMaterial).toHaveBeenCalledTimes(1);
  });
});
