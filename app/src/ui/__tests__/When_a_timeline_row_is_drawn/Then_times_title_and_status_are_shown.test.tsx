import { render, screen } from "@testing-library/react-native";
import { ThemeProvider } from "@/theme/ThemeProvider";
import { TimelineRow } from "@/ui/TimelineRow";

jest.mock("@/ui/motion/useReducedMotion", () => ({ useReducedMotion: () => true }));

describe("When a timeline row is drawn", () => {
  it("Then times, title and status are shown", async () => {
    await render(
      <ThemeProvider initialColorSchemePreference="light">
        <TimelineRow
          startTime="10:00"
          endTime="11:00"
          title="Nivel inicial"
          meta="8 alumnos"
          status="En curso"
          detail="6 de 8"
          tone="live"
          isFirst
          isLast={false}
          onPress={() => undefined}
          accessibilityLabel="10:00 Nivel inicial"
        />
      </ThemeProvider>,
    );

    const row = screen.getByRole("button", { name: "10:00 Nivel inicial" });
    expect(row).toHaveTextContent("10:0011:00Nivel inicial8 alumnosEn curso6 de 8");
  });
});
