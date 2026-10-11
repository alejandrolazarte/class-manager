import { render, screen } from "@testing-library/react-native";
import { ThemeProvider } from "@/theme/ThemeProvider";
import { Notice } from "@/ui/Notice";

describe("When a notice is shown", () => {
  it("Then its message is shown", async () => {
    await render(
      <ThemeProvider initialColorSchemePreference="light">
        <Notice tone="warning" message="Se fabrica a pedido" />
      </ThemeProvider>,
    );

    expect(screen.getByText("Se fabrica a pedido")).toBeOnTheScreen();
  });
});
