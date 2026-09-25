import { useQuery } from "@tanstack/react-query";
import { enrollmentQueryKeys } from "@/features/enrollments/enrollmentQueryKeys";
import { listClassRoster } from "@/features/enrollments/enrollmentsApi";

export function useClassRoster(classGroupId: string) {
  return useQuery({
    queryKey: enrollmentQueryKeys.roster(classGroupId),
    queryFn: () => listClassRoster(classGroupId),
  });
}
