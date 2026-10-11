import { render, screen } from "@testing-library/react-native";
import { ThemeProvider } from "@/theme/ThemeProvider";
import { ConfirmDialog } from "@/ui/ConfirmDialog";

jest.mock("@/ui/motion/useReducedMotion", () => ({ useReducedMotion: () => true }));

describe("When reduced motion is on", () => {
  it("Then a confirm dialog shows its question", async () => {
    await render(
      <ThemeProvider initialColorSchemePreference="light">
        <ConfirmDialog
          title="¿Dar de baja?"
          confirmLabel="Dar de baja"
          onCancel={() => undefined}
          onConfirm={() => undefined}
        />
      </ThemeProvider>,
    );

    expect(screen.getByRole("header", { name: "¿Dar de baja?" })).toBeTruthy();
  });
});
