import { planInstructorDay } from "@/features/privateLessons/instructorDay";
import { buildInstructorAgendaSlot } from "@/testing/privateLessonFactory";

describe("When the instructor has classes that day", () => {
  it("Then only gaps that fit the duration are offered", () => {
    const slots = [
      buildInstructorAgendaSlot({ startTime: "09:00", durationMinutes: 60 }),
      buildInstructorAgendaSlot({ startTime: "10:30", durationMinutes: 60 }),
      buildInstructorAgendaSlot({ startTime: "13:00", durationMinutes: 60 }),
    ];

    const instructorDay = planInstructorDay(slots, 60, "");

    expect(instructorDay.freeGaps.map((gap) => [gap.startMinutes, gap.endMinutes])).toEqual([
      [0, 540],
      [690, 780],
      [840, 1440],
    ]);
  });
});
