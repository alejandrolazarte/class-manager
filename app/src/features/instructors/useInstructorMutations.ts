import { useMutation, useQueryClient } from "@tanstack/react-query";
import { classGroupQueryKeys } from "@/features/classGroups/classGroupQueryKeys";
import { instructorQueryKeys } from "@/features/instructors/instructorQueryKeys";
import {
  createInstructor,
  renameInstructor,
  setInstructorActive,
} from "@/features/instructors/instructorsApi";
import { SaveInstructorRequest } from "@/features/instructors/types";

interface SaveInstructorVariables {
  instructorId?: string;
  request: SaveInstructorRequest;
}

interface SetInstructorActiveVariables {
  instructorId: string;
  isActive: boolean;
}

function useInvalidateInstructorsAndClasses() {
  const queryClient = useQueryClient();
  return () =>
    Promise.all([
      queryClient.invalidateQueries({ queryKey: instructorQueryKeys.all }),
      queryClient.invalidateQueries({ queryKey: classGroupQueryKeys.all }),
    ]);
}

export function useSaveInstructor() {
  const invalidate = useInvalidateInstructorsAndClasses();
  return useMutation({
    mutationFn: ({ instructorId, request }: SaveInstructorVariables) =>
      instructorId === undefined
        ? createInstructor(request)
        : renameInstructor(instructorId, request),
    onSuccess: invalidate,
  });
}

export function useSetInstructorActive() {
  const invalidate = useInvalidateInstructorsAndClasses();
  return useMutation({
    mutationFn: ({ instructorId, isActive }: SetInstructorActiveVariables) =>
      setInstructorActive(instructorId, isActive),
    onSuccess: invalidate,
  });
}
