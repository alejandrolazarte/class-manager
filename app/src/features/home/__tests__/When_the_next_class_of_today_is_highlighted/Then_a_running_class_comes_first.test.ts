import { todayHighlightOf } from "@/features/home/agenda";
import { buildDaySession } from "@/testing/sessionFactory";

const today = "2026-10-01";
const morning = buildDaySession({
  date: today,
  classGroupName: "Mañana",
  startTime: "09:00",
  endTime: "10:00",
});
const running = buildDaySession({
  date: today,
  classGroupName: "Ahora",
  startTime: "17:00",
  endTime: "18:00",
});
const evening = buildDaySession({
  date: today,
  classGroupName: "Noche",
  startTime: "19:00",
  endTime: "20:00",
});

describe("When the next class of today is highlighted", () => {
  it("Then a running class comes first", () => {
    expect(todayHighlightOf([evening, running, morning], today, "17:20")).toEqual({
      kind: "live",
      session: running,
    });
    expect(todayHighlightOf([evening, running, morning], today, "18:10")).toEqual({
      kind: "next",
      session: evening,
    });
    expect(todayHighlightOf([evening, running, morning], today, "20:00")).toEqual({ kind: "none" });
  });
});
