import { fireEvent, render, screen } from "@testing-library/react-native";
import { ThemeProvider } from "@/theme/ThemeProvider";
import { MonthCalendar, MonthCalendarDay } from "@/ui/MonthCalendar";

const days: MonthCalendarDay[] = Array.from({ length: 7 }, (_, index) => ({
  key: `2026-10-${String(index + 12).padStart(2, "0")}`,
  dayNumber: index + 12,
  accessibilityLabel: `Día ${index + 12}`,
  isSelected: index === 1,
  isToday: index === 1,
  isOutside: false,
  isCancelled: index === 3,
  marker: index === 2 ? "pending" : "classes",
}));

describe("When a month day is picked", () => {
  it("Then the day is selected", async () => {
    const select = jest.fn();
    await render(
      <ThemeProvider initialColorSchemePreference="light">
        <MonthCalendar
          weekdayLabels={["Lun", "Mar", "Mié", "Jue", "Vie", "Sáb", "Dom"]}
          days={days}
          onSelect={select}
        />
      </ThemeProvider>,
    );

    await fireEvent.press(screen.getByRole("button", { name: "Día 15" }));

    expect(select).toHaveBeenCalledWith("2026-10-15");
  });
});
