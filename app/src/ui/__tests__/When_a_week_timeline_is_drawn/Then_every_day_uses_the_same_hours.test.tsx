import { render, screen } from "@testing-library/react-native";
import { StyleSheet } from "react-native";
import { ThemeProvider } from "@/theme/ThemeProvider";
import { WeekTimeline } from "@/ui/DayTimeline";

const legendLabels = { busy: "Ocupado", free: "Libre", chosen: "Esta clase", conflict: "Choca" };

describe("When a week timeline is drawn", () => {
  it("Then every day uses the same hours", async () => {
    await render(
      <ThemeProvider initialColorSchemePreference="light">
        <WeekTimeline
          startMinutes={600}
          endMinutes={1200}
          rows={[
            {
              key: "Monday",
              label: "Lun",
              periods: [{ startMinutes: 660, endMinutes: 720, tone: "busy" }],
              accessibilityLabel: "Lunes",
            },
            {
              key: "Wednesday",
              label: "Mié",
              periods: [{ startMinutes: 900, endMinutes: 960, tone: "busy" }],
              accessibilityLabel: "Miércoles",
            },
          ]}
          legendLabels={legendLabels}
        />
      </ThemeProvider>,
    );

    const periodLefts = screen
      .getAllByTestId("day-timeline-period-busy")
      .map((period) => StyleSheet.flatten(period.props.style).left);
    expect(periodLefts).toEqual(["10%", "50%"]);
  });
});
