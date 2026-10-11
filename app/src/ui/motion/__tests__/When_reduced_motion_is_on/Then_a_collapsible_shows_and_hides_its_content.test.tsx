import { render, screen } from "@testing-library/react-native";
import { ThemeProvider } from "@/theme/ThemeProvider";
import { AppText } from "@/ui/AppText";
import { Collapsible } from "@/ui/motion/Collapsible";

jest.mock("@/ui/motion/useReducedMotion", () => ({ useReducedMotion: () => true }));

function Panel({ isExpanded }: { isExpanded: boolean }) {
  return (
    <ThemeProvider initialColorSchemePreference="light">
      <Collapsible isExpanded={isExpanded}>
        <AppText>Cambiar horario</AppText>
      </Collapsible>
    </ThemeProvider>
  );
}

describe("When reduced motion is on", () => {
  it("Then a collapsible shows and hides its content", async () => {
    const { rerender } = await render(<Panel isExpanded={false} />);
    expect(screen.queryByText("Cambiar horario")).toBeNull();

    await rerender(<Panel isExpanded />);
    expect(screen.getByText("Cambiar horario")).toBeTruthy();

    await rerender(<Panel isExpanded={false} />);
    expect(screen.queryByText("Cambiar horario")).toBeNull();
  });
});
