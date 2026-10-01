import { sessionTimingOf } from "@/features/home/agenda";
import { buildDaySession } from "@/testing/sessionFactory";

const today = "2026-10-01";
const sessionToday = buildDaySession({ date: today, startTime: "17:00", endTime: "18:00" });

describe("When the agenda status of a class is worked out", () => {
  it("Then it follows the clock", () => {
    expect(sessionTimingOf(sessionToday, today, "16:59")).toBe("upcoming");
    expect(sessionTimingOf(sessionToday, today, "17:00")).toBe("live");
    expect(sessionTimingOf(sessionToday, today, "18:00")).toBe("ended");
    expect(sessionTimingOf({ ...sessionToday, isCancelled: true }, today, "17:30")).toBe(
      "cancelled",
    );
    expect(sessionTimingOf({ ...sessionToday, date: "2026-09-30" }, today, "08:00")).toBe("ended");
    expect(sessionTimingOf({ ...sessionToday, date: "2026-10-02" }, today, "23:00")).toBe(
      "upcoming",
    );
  });
});
