import { useMutation, useQueryClient } from "@tanstack/react-query";
import { classGroupQueryKeys } from "@/features/classGroups/classGroupQueryKeys";
import { clientQueryKeys } from "@/features/clients/clientQueryKeys";
import { enrollmentQueryKeys } from "@/features/enrollments/enrollmentQueryKeys";
import { feeQueryKeys } from "@/features/fees/feeQueryKeys";
import { sessionQueryKeys } from "@/features/sessions/sessionQueryKeys";
import { deleteStudent } from "@/features/students/studentsApi";
import { studentQueryKeys } from "@/features/students/studentQueryKeys";

export function useDeleteStudent() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (studentId: string) => deleteStudent(studentId),
    onSuccess: () =>
      Promise.all([
        queryClient.invalidateQueries({ queryKey: clientQueryKeys.all }),
        queryClient.invalidateQueries({ queryKey: studentQueryKeys.all }),
        queryClient.invalidateQueries({ queryKey: enrollmentQueryKeys.all }),
        queryClient.invalidateQueries({ queryKey: classGroupQueryKeys.all }),
        queryClient.invalidateQueries({ queryKey: feeQueryKeys.all }),
        queryClient.invalidateQueries({ queryKey: sessionQueryKeys.all }),
      ]),
  });
}
