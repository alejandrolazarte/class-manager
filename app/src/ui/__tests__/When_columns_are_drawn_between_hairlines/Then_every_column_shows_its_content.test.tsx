import { render, screen } from "@testing-library/react-native";
import { ThemeProvider } from "@/theme/ThemeProvider";
import { AppText } from "@/ui/AppText";
import { HairlineColumns } from "@/ui/HairlineColumns";

describe("When columns are drawn between hairlines", () => {
  it("Then every column shows its content", async () => {
    await render(
      <ThemeProvider initialColorSchemePreference="light">
        <HairlineColumns>
          <AppText>Racha</AppText>
          {null}
          <AppText>Cuota del mes</AppText>
        </HairlineColumns>
      </ThemeProvider>,
    );

    expect(screen.getByText("Racha")).toBeOnTheScreen();
    expect(screen.getByText("Cuota del mes")).toBeOnTheScreen();
  });
});
