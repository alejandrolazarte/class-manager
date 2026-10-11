import { render, screen } from "@testing-library/react-native";
import { ThemeProvider } from "@/theme/ThemeProvider";
import { StatusText } from "@/ui/StatusText";

describe("When a status is written", () => {
  it("Then it takes the tone color", async () => {
    await render(
      <ThemeProvider initialColorSchemePreference="light">
        <StatusText label="Debe $25.000" tone="danger" />
      </ThemeProvider>,
    );

    expect(screen.getByText("Debe $25.000").props.className).toContain("text-danger");
  });
});
