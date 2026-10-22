import { render, screen } from "@testing-library/react-native";
import { ThemeProvider } from "@/theme/ThemeProvider";
import { TextField } from "@/ui/TextField";

const label = "Google Maps link";

describe("When a text field hides its label", () => {
  it("Then it keeps its accessible name", async () => {
    await render(
      <ThemeProvider initialColorSchemePreference="light">
        <TextField label={label} isLabelHidden value="" />
      </ThemeProvider>,
    );

    expect(screen.getByLabelText(label)).toBeOnTheScreen();
    expect(screen.queryByText(label)).toBeNull();
  });
});
