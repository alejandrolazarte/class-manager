import { render, screen } from "@testing-library/react-native";
import { ThemeProvider } from "@/theme/ThemeProvider";
import { Button } from "@/ui/Button";

describe("When a button toggles a section", () => {
  it("Then it says the section is expanded", async () => {
    await render(
      <ThemeProvider initialColorSchemePreference="light">
        <Button variant="ghost" size="medium" label="Cobrar" isExpanded onPress={jest.fn()} />
      </ThemeProvider>,
    );

    expect(screen.getByRole("button", { name: "Cobrar", expanded: true })).toBeOnTheScreen();
  });
});
