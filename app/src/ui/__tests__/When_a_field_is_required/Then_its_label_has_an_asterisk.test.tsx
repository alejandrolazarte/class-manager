import { render, screen } from "@testing-library/react-native";
import { ThemeProvider } from "@/theme/ThemeProvider";
import { TextField } from "@/ui/TextField";

describe("When a field is required", () => {
  it("Then its label has an asterisk", async () => {
    await render(
      <ThemeProvider initialColorSchemePreference="light">
        <TextField label="Nombre" isRequired value="" onChangeText={() => undefined} />
      </ThemeProvider>,
    );

    expect(screen.getByText("Nombre *")).toBeOnTheScreen();
    expect(screen.getByLabelText("Nombre")).toBeOnTheScreen();
  });
});
