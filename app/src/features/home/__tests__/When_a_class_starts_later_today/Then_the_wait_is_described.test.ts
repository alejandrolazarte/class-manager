import { relativeDayLabel, startsInLabel } from "@/features/home/agenda";

describe("When a class starts later today", () => {
  it("Then the wait is described", () => {
    expect(startsInLabel("14:40", "14:11")).toBe("en 29 min");
    expect(startsInLabel("17:11", "14:11")).toBe("en 3 h");
    expect(startsInLabel("17:00", "14:11")).toBe("en 2 h 49 min");
    expect(relativeDayLabel("2026-10-04", "2026-10-01")).toBe("En 3 días");
    expect(relativeDayLabel("2026-09-28", "2026-10-01")).toBe("Hace 3 días");
  });
});
