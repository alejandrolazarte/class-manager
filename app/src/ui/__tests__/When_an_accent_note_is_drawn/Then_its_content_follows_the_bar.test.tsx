import { render, screen } from "@testing-library/react-native";
import { ThemeProvider } from "@/theme/ThemeProvider";
import { AccentNote } from "@/ui/AccentNote";
import { AppText } from "@/ui/AppText";

describe("When an accent note is drawn", () => {
  it("Then its content follows the bar", async () => {
    await render(
      <ThemeProvider initialColorSchemePreference="light">
        <AccentNote tone="warning">
          <AppText>Tenés 1 clase para recuperar</AppText>
        </AccentNote>
      </ThemeProvider>,
    );

    expect(screen.getByTestId("accent-note-bar")).toBeOnTheScreen();
    expect(screen.getByText("Tenés 1 clase para recuperar")).toBeOnTheScreen();
  });
});
