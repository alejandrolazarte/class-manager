import { Instructor } from "@/features/instructors/types";
import { Team } from "@/features/members/types";

export function freeInstructors(
  instructors: Instructor[],
  team: Team | undefined,
  keptInstructorId: string | null = null,
): Instructor[] {
  const linkedInstructorIds = new Set(
    (team?.members ?? [])
      .map((member) => member.instructorId)
      .filter((instructorId) => instructorId !== null && instructorId !== keptInstructorId),
  );
  return instructors.filter((instructor) => !linkedInstructorIds.has(instructor.id));
}
