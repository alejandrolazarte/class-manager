import { useMutation, useQueryClient } from "@tanstack/react-query";
import { classGroupQueryKeys } from "@/features/classGroups/classGroupQueryKeys";
import { enrollmentQueryKeys } from "@/features/enrollments/enrollmentQueryKeys";
import { endEnrollment, enrollStudent } from "@/features/enrollments/enrollmentsApi";
import { EnrollStudentRequest } from "@/features/enrollments/types";
import { feeQueryKeys } from "@/features/fees/feeQueryKeys";
import { sessionQueryKeys } from "@/features/sessions/sessionQueryKeys";

function useInvalidateEnrollments() {
  const queryClient = useQueryClient();
  return () =>
    Promise.all([
      queryClient.invalidateQueries({ queryKey: enrollmentQueryKeys.all }),
      queryClient.invalidateQueries({ queryKey: classGroupQueryKeys.all }),
      queryClient.invalidateQueries({ queryKey: feeQueryKeys.all }),
      queryClient.invalidateQueries({ queryKey: sessionQueryKeys.all }),
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
