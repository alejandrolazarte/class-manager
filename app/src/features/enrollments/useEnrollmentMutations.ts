import { useMutation, useQueryClient } from "@tanstack/react-query";
import { classGroupQueryKeys } from "@/features/classGroups/classGroupQueryKeys";
import { enrollmentQueryKeys } from "@/features/enrollments/enrollmentQueryKeys";
import { endEnrollment, enrollStudent } from "@/features/enrollments/enrollmentsApi";
import { EnrollStudentRequest } from "@/features/enrollments/types";

function useInvalidateEnrollments() {
  const queryClient = useQueryClient();
  return () =>
    Promise.all([
      queryClient.invalidateQueries({ queryKey: enrollmentQueryKeys.all }),
      queryClient.invalidateQueries({ queryKey: classGroupQueryKeys.all }),
    ]);
}

export function useEnrollStudent(classGroupId: string) {
  const invalidateEnrollments = useInvalidateEnrollments();
  return useMutation({
    mutationFn: (request: EnrollStudentRequest) => enrollStudent(classGroupId, request),
    onSuccess: invalidateEnrollments,
  });
}

export function useEndEnrollment() {
  const invalidateEnrollments = useInvalidateEnrollments();
  return useMutation({
    mutationFn: (enrollmentId: string) => endEnrollment(enrollmentId, {}),
    onSuccess: invalidateEnrollments,
  });
}
