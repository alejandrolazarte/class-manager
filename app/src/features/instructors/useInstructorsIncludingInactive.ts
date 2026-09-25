import { useQuery } from "@tanstack/react-query";
import { instructorQueryKeys } from "@/features/instructors/instructorQueryKeys";
import { listInstructorsIncludingInactive } from "@/features/instructors/instructorsApi";
import { Instructor } from "@/features/instructors/types";
import { sortActiveFirstByName } from "@/features/settings/sortActiveFirstByName";

export function useInstructorsIncludingInactive({ enabled = true }: { enabled?: boolean } = {}) {
  return useQuery({
    enabled,
    queryKey: instructorQueryKeys.includingInactive,
    queryFn: listInstructorsIncludingInactive,
    select: (instructors: Instructor[]) =>
      sortActiveFirstByName(instructors, (instructor) => instructor.fullName),
  });
}
