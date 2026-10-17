import { addMonthsToDate } from "@/features/sessions/dates";

describe("When a date moves to a shorter month", () => {
  it("Then it lands on the last day of the month", () => {
    expect(addMonthsToDate("2026-01-31", 1)).toBe("2026-02-28");
  });
});
