import { render, screen } from "@testing-library/react-native";
import { ThemeProvider } from "@/theme/ThemeProvider";
import { AppText } from "@/ui/AppText";
import { LivePulse } from "@/ui/motion/LivePulse";

jest.mock("@/ui/motion/useReducedMotion", () => ({ useReducedMotion: () => true }));

describe("When reduced motion is on", () => {
  it("Then the live pulse shows its content", async () => {
    await render(
      <ThemeProvider initialColorSchemePreference="light">
        <LivePulse ringClassName="rounded-xl bg-primary">
          <AppText>En curso</AppText>
        </LivePulse>
      </ThemeProvider>,
    );

    expect(screen.getByText("En curso")).toBeTruthy();
  });
});
