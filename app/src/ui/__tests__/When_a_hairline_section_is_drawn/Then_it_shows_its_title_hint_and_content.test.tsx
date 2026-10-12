import { render, screen } from "@testing-library/react-native";
import { ThemeProvider } from "@/theme/ThemeProvider";
import { AppText } from "@/ui/AppText";
import { HairlineSection } from "@/ui/HairlineSection";

describe("When a hairline section is drawn", () => {
  it("Then it shows its title, hint and content", async () => {
    await render(
      <ThemeProvider initialColorSchemePreference="light">
        <HairlineSection title="Color principal" hint="Se usa en botones">
          <AppText>Muestras</AppText>
        </HairlineSection>
      </ThemeProvider>,
    );

    expect(screen.getByText("Color principal")).toBeOnTheScreen();
    expect(screen.getByText("Se usa en botones")).toBeOnTheScreen();
    expect(screen.getByText("Muestras")).toBeOnTheScreen();
  });
});
