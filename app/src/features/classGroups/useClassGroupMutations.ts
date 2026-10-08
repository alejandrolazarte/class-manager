import { useMutation, useQueryClient } from "@tanstack/react-query";
import { PickedFile } from "@/api/fileForm";
import { classGroupQueryKeys } from "@/features/classGroups/classGroupQueryKeys";
import {
  createClassGroup,
  removeClassMaterialFile,
  setClassGroupActive,
  updateClassGroup,
  uploadClassMaterialFile,
} from "@/features/classGroups/classGroupsApi";
import { SaveClassGroupRequest } from "@/features/classGroups/types";

export type ClassMaterialFileChange =
  { kind: "unchanged" } | { kind: "picked"; file: PickedFile } | { kind: "removed" };

export type SaveClassGroupOutcome = "saved" | "savedWithoutMaterialFile";

interface SaveClassGroupVariables {
  classGroupId?: string;
  request: SaveClassGroupRequest;
  materialFileChange?: ClassMaterialFileChange;
}

const unchangedMaterialFile: ClassMaterialFileChange = { kind: "unchanged" };

async function saveClassGroup({
  classGroupId,
  request,
  materialFileChange = unchangedMaterialFile,
}: SaveClassGroupVariables): Promise<SaveClassGroupOutcome> {
  if (classGroupId !== undefined && materialFileChange.kind === "removed") {
    await removeClassMaterialFile(classGroupId);
  }
  const savedClassGroup =
    classGroupId === undefined
      ? await createClassGroup(request)
      : await updateClassGroup(classGroupId, request);
  if (materialFileChange.kind !== "picked") {
    return "saved";
  }
  try {
    await uploadClassMaterialFile(savedClassGroup.id, materialFileChange.file);
    return "saved";
  } catch {
    return "savedWithoutMaterialFile";
  }
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
    mutationFn: saveClassGroup,
    onSettled: invalidateClassGroups,
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
