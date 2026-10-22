import { render, screen } from "@testing-library/react-native";
import { ThemeProvider } from "@/theme/ThemeProvider";
import { NumberedStep } from "@/ui/NumberedStep";

describe("When a numbered step is drawn", () => {
  it("Then number, title and detail are shown", async () => {
    await render(
      <ThemeProvider initialColorSchemePreference="light">
        <NumberedStep number={2} title="Tocá Compartir" detail="Debajo del nombre." />
      </ThemeProvider>,
    );

    expect(screen.getByText("2")).toBeOnTheScreen();
    expect(screen.getByText("Tocá Compartir")).toBeOnTheScreen();
    expect(screen.getByText("Debajo del nombre.")).toBeOnTheScreen();
  });
});
