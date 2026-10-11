import { render, screen } from "@testing-library/react-native";
import { ThemeProvider } from "@/theme/ThemeProvider";
import { OptionCard } from "@/ui/OptionCard";

describe("When an option is drawn", () => {
  it("Then a selected option is announced as checked", async () => {
    await render(
      <ThemeProvider initialColorSchemePreference="light">
        <OptionCard label="Retiro en la sede" isSelected onPress={() => undefined} />
        <OptionCard label="En la clase" isSelected={false} onPress={() => undefined} />
      </ThemeProvider>,
    );

    expect(screen.getByRole("radio", { name: "Retiro en la sede" })).toBeChecked();
    expect(screen.getByRole("radio", { name: "En la clase" })).not.toBeChecked();
  });
});
