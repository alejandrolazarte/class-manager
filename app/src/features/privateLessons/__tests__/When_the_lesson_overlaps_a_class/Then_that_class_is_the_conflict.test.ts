import { planInstructorDay } from "@/features/privateLessons/instructorDay";
import { buildInstructorAgendaSlot } from "@/testing/privateLessonFactory";

describe("When the lesson overlaps a class", () => {
  it("Then that class is the conflict", () => {
    const overlappedSlot = buildInstructorAgendaSlot({ startTime: "16:00", durationMinutes: 60 });
    const slots = [buildInstructorAgendaSlot({ startTime: "08:00" }), overlappedSlot];

    const instructorDay = planInstructorDay(slots, 45, "16:30");

    expect(instructorDay.lesson?.conflict?.slot).toBe(overlappedSlot);
  });
});
