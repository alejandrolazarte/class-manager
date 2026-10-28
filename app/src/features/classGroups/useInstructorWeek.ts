import { useQuery } from "@tanstack/react-query";
import { classGroupQueryKeys } from "@/features/classGroups/classGroupQueryKeys";
import { listInstructorWeek } from "@/features/classGroups/classGroupsApi";

export function useInstructorWeek(instructorId: string, ignoredClassGroupId: string | undefined) {
  return useQuery({
    queryKey: classGroupQueryKeys.instructorWeek(instructorId, ignoredClassGroupId ?? ""),
    queryFn: () => listInstructorWeek(instructorId, ignoredClassGroupId),
    enabled: instructorId.length > 0,
  });
}
