import { nextSessionOf } from "@/features/home/nextSession";
import { buildDaySession } from "@/testing/sessionFactory";

describe("When the next class is picked", () => {
  it("Then finished and cancelled classes are skipped", () => {
    const finished = buildDaySession({
      classGroupName: "Temprano",
      startTime: "08:00",
      endTime: "09:00",
    });
    const cancelled = buildDaySession({
      classGroupName: "Cancelada",
      startTime: "10:00",
      endTime: "11:00",
      isCancelled: true,
    });
    const later = buildDaySession({
      classGroupName: "Tarde",
      startTime: "18:00",
      endTime: "19:00",
    });
    const inProgress = buildDaySession({
      classGroupName: "Ahora",
      startTime: "09:30",
      endTime: "10:30",
    });

    expect(nextSessionOf([later, finished, cancelled, inProgress], "10:00")?.classGroupName).toBe(
      "Ahora",
    );
  });
});
