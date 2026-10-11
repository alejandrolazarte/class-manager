import { render, screen } from "@testing-library/react-native";
import { ThemeProvider } from "@/theme/ThemeProvider";
import { AppText } from "@/ui/AppText";
import { BottomSheet } from "@/ui/BottomSheet";

jest.mock("@/ui/motion/useReducedMotion", () => ({ useReducedMotion: () => true }));

describe("When reduced motion is on", () => {
  it("Then a bottom sheet shows its content", async () => {
    await render(
      <ThemeProvider initialColorSchemePreference="light">
        <BottomSheet onClose={() => undefined}>
          <AppText>Apariencia</AppText>
        </BottomSheet>
      </ThemeProvider>,
    );

    expect(screen.getByText("Apariencia")).toBeTruthy();
  });
});
