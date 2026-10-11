import { render, screen } from "@testing-library/react-native";
import { ThemeProvider } from "@/theme/ThemeProvider";
import { AppText } from "@/ui/AppText";
import { InsetPanel } from "@/ui/InsetPanel";

describe("When an inset panel is drawn", () => {
  it("Then it shows its content", async () => {
    await render(
      <ThemeProvider initialColorSchemePreference="light">
        <InsetPanel>
          <AppText>Forma de pago</AppText>
        </InsetPanel>
      </ThemeProvider>,
    );

    expect(screen.getByText("Forma de pago")).toBeOnTheScreen();
  });
});
