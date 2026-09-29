import { useQuery } from "@tanstack/react-query";
import { instructorQueryKeys } from "@/features/instructors/instructorQueryKeys";
import { listActiveInstructors } from "@/features/instructors/instructorsApi";

export function useActiveInstructors({ enabled = true }: { enabled?: boolean } = {}) {
  return useQuery({
    enabled,
    queryKey: instructorQueryKeys.active,
    queryFn: listActiveInstructors,
  });
}
