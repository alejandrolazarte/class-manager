import { useMutation, useQueryClient } from "@tanstack/react-query";
import { clientQueryKeys } from "@/features/clients/clientQueryKeys";
import { feeQueryKeys } from "@/features/fees/feeQueryKeys";
import { updateStudent } from "@/features/students/studentsApi";
import { studentQueryKeys } from "@/features/students/studentQueryKeys";
import { UpdateStudentRequest } from "@/features/students/types";

export function useUpdateStudent(studentId: string) {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (request: UpdateStudentRequest) => updateStudent(studentId, request),
    onSuccess: () =>
      Promise.all([
        queryClient.invalidateQueries({ queryKey: clientQueryKeys.all }),
        queryClient.invalidateQueries({ queryKey: studentQueryKeys.all }),
        queryClient.invalidateQueries({ queryKey: feeQueryKeys.all }),
      ]),
  });
}
