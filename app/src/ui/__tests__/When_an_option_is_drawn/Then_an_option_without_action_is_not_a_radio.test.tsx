import { render, screen } from "@testing-library/react-native";
import { ThemeProvider } from "@/theme/ThemeProvider";
import { OptionCard } from "@/ui/OptionCard";

describe("When an option is drawn", () => {
  it("Then an option without action is not a radio", async () => {
    await render(
      <ThemeProvider initialColorSchemePreference="light">
        <OptionCard label="En la sede" hint="Tenés 7 días para pagarlo" isSelected />
      </ThemeProvider>,
    );

    expect(screen.getByText("En la sede")).toBeOnTheScreen();
    expect(screen.queryByRole("radio")).toBeNull();
  });
});
