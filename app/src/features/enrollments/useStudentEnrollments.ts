import { useQuery } from "@tanstack/react-query";
import { enrollmentQueryKeys } from "@/features/enrollments/enrollmentQueryKeys";
import { listStudentEnrollments } from "@/features/enrollments/enrollmentsApi";

export function useStudentEnrollments(studentId: string) {
  return useQuery({
    queryKey: enrollmentQueryKeys.student(studentId),
    queryFn: () => listStudentEnrollments(studentId),
  });
}
