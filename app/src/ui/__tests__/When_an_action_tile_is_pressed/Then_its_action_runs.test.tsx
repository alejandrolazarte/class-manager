import { fireEvent, render, screen } from "@testing-library/react-native";
import { ThemeProvider } from "@/theme/ThemeProvider";
import { ActionTile } from "@/ui/ActionTile";

jest.mock("@/ui/motion/useReducedMotion", () => ({ useReducedMotion: () => true }));

describe("When an action tile is pressed", () => {
  it("Then its action runs", async () => {
    const call = jest.fn();
    await render(
      <ThemeProvider initialColorSchemePreference="light">
        <ActionTile icon="call" label="Llamar" onPress={call} />
      </ThemeProvider>,
    );

    await fireEvent.press(screen.getByRole("button", { name: "Llamar" }));

    expect(call).toHaveBeenCalledTimes(1);
  });
});
