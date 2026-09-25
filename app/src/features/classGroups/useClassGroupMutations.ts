import { useMutation, useQueryClient } from "@tanstack/react-query";
import { classGroupQueryKeys } from "@/features/classGroups/classGroupQueryKeys";
import {
  createClassGroup,
  setClassGroupActive,
  updateClassGroup,
} from "@/features/classGroups/classGroupsApi";
import { SaveClassGroupRequest } from "@/features/classGroups/types";

interface SaveClassGroupVariables {
  classGroupId?: string;
  request: SaveClassGroupRequest;
}

interface SetClassGroupActiveVariables {
  classGroupId: string;
  isActive: boolean;
}

function useInvalidateClassGroups() {
  const queryClient = useQueryClient();
  return () => queryClient.invalidateQueries({ queryKey: classGroupQueryKeys.all });
}

export function useSaveClassGroup() {
  const invalidateClassGroups = useInvalidateClassGroups();
  return useMutation({
    mutationFn: ({ classGroupId, request }: SaveClassGroupVariables) =>
      classGroupId === undefined
        ? createClassGroup(request)
        : updateClassGroup(classGroupId, request),
    onSuccess: invalidateClassGroups,
  });
}

export function useSetClassGroupActive() {
  const invalidateClassGroups = useInvalidateClassGroups();
  return useMutation({
    mutationFn: ({ classGroupId, isActive }: SetClassGroupActiveVariables) =>
      setClassGroupActive(classGroupId, isActive),
    onSuccess: invalidateClassGroups,
  });
}
