import { render, screen } from "@testing-library/react-native";
import { ThemeProvider } from "@/theme/ThemeProvider";
import { ProgressSteps } from "@/ui/ProgressSteps";

describe("When progress steps are drawn", () => {
  it("Then steps up to the current one are reached", async () => {
    await render(
      <ThemeProvider initialColorSchemePreference="light">
        <ProgressSteps steps={["Pedido", "Pagado", "Listo", "Entregado"]} currentStep={1} />
      </ThemeProvider>,
    );

    expect(screen.getAllByTestId("progress-step-reached")).toHaveLength(2);
    expect(screen.getAllByTestId("progress-step-pending")).toHaveLength(2);
    expect(screen.getByText("Entregado")).toBeOnTheScreen();
  });
});
