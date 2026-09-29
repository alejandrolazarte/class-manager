import { everyPermission } from "@/features/members/permissions";
import { coachPermissions, viewerPermissions } from "@/testing/memberFactory";
import { Role } from "@/features/roles/types";

export function buildSystemRoles(): Role[] {
  return [
    {
      id: null,
      systemRole: "BranchOwner",
      name: null,
      permissions: everyPermission.filter(
        (permission) =>
          !["branchOwners.manage", "branches.create", "brandOwners.manage"].includes(permission),
      ),
      copiedFrom: null,
      memberCount: 1,
    },
    {
      id: null,
      systemRole: "Coach",
      name: null,
      permissions: [...coachPermissions],
      copiedFrom: null,
      memberCount: 1,
    },
    {
      id: null,
      systemRole: "Viewer",
      name: null,
      permissions: [...viewerPermissions],
      copiedFrom: null,
      memberCount: 0,
    },
  ];
}

export function buildCustomRole(overrides: Partial<Role> = {}): Role {
  return {
    id: "role-custom",
    systemRole: null,
    name: "Coach que cobra",
    permissions: [...coachPermissions, "payments.view.all", "payments.record"],
    copiedFrom: "Coach",
    memberCount: 0,
    ...overrides,
  };
}
