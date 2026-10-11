import { fireEvent, render, screen } from "@testing-library/react-native";
import { ThemeProvider } from "@/theme/ThemeProvider";
import { Button } from "@/ui/Button";
import { ToastProvider, useToast } from "@/ui/ToastProvider";

jest.mock("@/ui/motion/useReducedMotion", () => ({ useReducedMotion: () => true }));

function ShowToastButton() {
  const { showToast } = useToast();
  return <Button label="Avisar" onPress={() => showToast("Pago borrado")} />;
}

describe("When reduced motion is on", () => {
  it("Then a toast shows its message", async () => {
    await render(
      <ThemeProvider initialColorSchemePreference="light">
        <ToastProvider>
          <ShowToastButton />
        </ToastProvider>
      </ThemeProvider>,
    );

    await fireEvent.press(screen.getByRole("button", { name: "Avisar" }));

    expect(screen.getByText("Pago borrado")).toBeTruthy();
  });
});
