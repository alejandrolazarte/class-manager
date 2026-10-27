import { planInstructorDay } from "@/features/privateLessons/instructorDay";
import { buildInstructorAgendaSlot } from "@/testing/privateLessonFactory";

describe("When the first free gap starts at midnight", () => {
  it("Then its suggested start ends at the first class", () => {
    const slots = [buildInstructorAgendaSlot({ startTime: "08:00", durationMinutes: 60 })];

    const instructorDay = planInstructorDay(slots, 45, "");

    expect(instructorDay.freeGaps[0]?.suggestedStartMinutes).toBe(435);
  });
});
