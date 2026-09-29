import { Permission } from "@/features/members/permissions";
import { SystemRole } from "@/features/members/types";

export interface Role {
  id: string | null;
  systemRole: SystemRole | null;
  name: string | null;
  permissions: Permission[];
  copiedFrom: string | null;
  memberCount: number;
}

export interface SaveRoleRequest {
  name: string;
  permissions: Permission[];
}

export interface CreateRoleRequest extends SaveRoleRequest {
  copiedFrom: string | null;
}
