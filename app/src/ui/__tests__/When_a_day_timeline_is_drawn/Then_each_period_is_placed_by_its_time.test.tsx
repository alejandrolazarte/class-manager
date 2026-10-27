import { render, screen } from "@testing-library/react-native";
import { StyleSheet } from "react-native";
import { ThemeProvider } from "@/theme/ThemeProvider";
import { DayTimeline } from "@/ui/DayTimeline";

const legendLabels = { busy: "Ocupado", free: "Libre", chosen: "Esta clase", conflict: "Choca" };

describe("When a day timeline is drawn", () => {
  it("Then each period is placed by its time", async () => {
    await render(
      <ThemeProvider initialColorSchemePreference="light">
        <DayTimeline
          startMinutes={600}
          endMinutes={1200}
          periods={[{ startMinutes: 660, endMinutes: 720, tone: "busy" }]}
          accessibilityLabel="Agenda"
          legendLabels={legendLabels}
        />
      </ThemeProvider>,
    );

    const periodStyle = StyleSheet.flatten(
      screen.getByTestId("day-timeline-period-busy").props.style,
    );
    expect([periodStyle.left, periodStyle.width]).toEqual(["10%", "10%"]);
  });
});
