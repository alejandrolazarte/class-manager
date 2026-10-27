import { planInstructorDay } from "@/features/privateLessons/instructorDay";
import { buildInstructorAgendaSlot } from "@/testing/privateLessonFactory";

describe("When the lesson is after the instructor's last class", () => {
  it("Then the visible day stretches to include it", () => {
    const slots = [buildInstructorAgendaSlot({ startTime: "08:00", durationMinutes: 60 })];

    const instructorDay = planInstructorDay(slots, 45, "21:30");

    expect([instructorDay.visibleStartMinutes, instructorDay.visibleEndMinutes]).toEqual([
      420, 1380,
    ]);
  });
});
