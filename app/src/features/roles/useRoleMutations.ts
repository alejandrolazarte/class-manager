import { useMutation, useQueryClient } from "@tanstack/react-query";
import { memberQueryKeys } from "@/features/members/memberQueryKeys";
import { roleQueryKeys } from "@/features/roles/roleQueryKeys";
import { createRole, deleteRole, updateRole } from "@/features/roles/rolesApi";
import { CreateRoleRequest, SaveRoleRequest } from "@/features/roles/types";

function useInvalidateRoles() {
  const queryClient = useQueryClient();
  return () =>
    Promise.all([
      queryClient.invalidateQueries({ queryKey: roleQueryKeys.all }),
      queryClient.invalidateQueries({ queryKey: memberQueryKeys.team() }),
    ]);
}

export function useCreateRole() {
  const invalidateRoles = useInvalidateRoles();
  return useMutation({
    mutationFn: (request: CreateRoleRequest) => createRole(request),
    onSuccess: invalidateRoles,
  });
}

export function useUpdateRole(roleId: string) {
  const invalidateRoles = useInvalidateRoles();
  return useMutation({
    mutationFn: (request: SaveRoleRequest) => updateRole(roleId, request),
    onSuccess: invalidateRoles,
  });
}

export function useDeleteRole() {
  const invalidateRoles = useInvalidateRoles();
  return useMutation({
    mutationFn: (roleId: string) => deleteRole(roleId),
    onSuccess: invalidateRoles,
  });
}
