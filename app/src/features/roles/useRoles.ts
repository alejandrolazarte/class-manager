import { useQuery } from "@tanstack/react-query";
import { roleQueryKeys } from "@/features/roles/roleQueryKeys";
import { listRoles } from "@/features/roles/rolesApi";

export function useRoles() {
  return useQuery({ queryKey: roleQueryKeys.all, queryFn: listRoles });
}
