import { fireEvent, render, screen } from "@testing-library/react-native";
import { ThemeProvider } from "@/theme/ThemeProvider";
import { Highlight } from "@/ui/Highlight";

jest.mock("@/ui/motion/useReducedMotion", () => ({ useReducedMotion: () => true }));

describe("When a highlight is pressed", () => {
  it("Then its action runs", async () => {
    const open = jest.fn();
    await render(
      <ThemeProvider initialColorSchemePreference="light">
        <Highlight
          isLive
          eyebrow="En curso · hasta 11:00"
          title="Nivel inicial"
          actionLabel="Asistencia"
          accessibilityLabel="Asistencia"
          onPress={open}
        />
      </ThemeProvider>,
    );

    await fireEvent.press(screen.getByRole("button", { name: "Asistencia" }));

    expect(open).toHaveBeenCalledTimes(1);
  });
});
