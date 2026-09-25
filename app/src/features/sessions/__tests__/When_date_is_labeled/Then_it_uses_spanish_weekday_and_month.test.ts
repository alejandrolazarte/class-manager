import { formatLongDate } from "@/features/sessions/dates";

describe("When date is labeled", () => {
  it("Then it uses spanish weekday and month", () => {
    expect(formatLongDate("2026-09-29")).toBe("Martes 29 de septiembre");
  });
});
