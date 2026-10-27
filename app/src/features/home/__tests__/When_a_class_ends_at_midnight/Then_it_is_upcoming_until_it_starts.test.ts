import { sessionTimingOf } from "@/features/home/agenda";
import { buildDaySession } from "@/testing/sessionFactory";

const today = "2026-10-27";
const sessionUntilMidnight = buildDaySession({ date: today, startTime: "23:30", endTime: "00:00" });

describe("When a class ends at midnight", () => {
  it("Then it is upcoming until it starts", () => {
    expect(sessionTimingOf(sessionUntilMidnight, today, "21:23")).toBe("upcoming");
    expect(sessionTimingOf(sessionUntilMidnight, today, "23:45")).toBe("live");
  });
});
