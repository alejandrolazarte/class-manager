import { useMutation, useQueryClient } from "@tanstack/react-query";
import { clientQueryKeys } from "@/features/clients/clientQueryKeys";
import { addStudent } from "@/features/students/studentsApi";
import { studentQueryKeys } from "@/features/students/studentQueryKeys";
import { NewStudentRequest } from "@/features/students/types";

export function useAddStudent(clientId: string) {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (request: NewStudentRequest) => addStudent(clientId, request),
    onSuccess: async () => {
      await Promise.all([
        queryClient.invalidateQueries({ queryKey: clientQueryKeys.detail(clientId) }),
        queryClient.invalidateQueries({ queryKey: studentQueryKeys.all, refetchType: "active" }),
      ]);
    },
  });
}
