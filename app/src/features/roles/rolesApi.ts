import { httpClient } from "@/api/httpClient";
import { CreateRoleRequest, Role, SaveRoleRequest } from "@/features/roles/types";

const rolesPath = "/api/roles";

export function listRoles(): Promise<Role[]> {
  return httpClient.get<Role[]>(rolesPath);
}

export function createRole(request: CreateRoleRequest): Promise<Role> {
  return httpClient.post<Role>(rolesPath, request);
}

export function updateRole(roleId: string, request: SaveRoleRequest): Promise<Role> {
  return httpClient.put<Role>(`${rolesPath}/${encodeURIComponent(roleId)}`, request);
}

export function deleteRole(roleId: string): Promise<void> {
  return httpClient.delete<void>(`${rolesPath}/${encodeURIComponent(roleId)}`);
}
