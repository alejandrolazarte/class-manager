import { everyPermission } from "@/features/members/permissions";
import { instructorPermissions, viewerPermissions } from "@/testing/memberFactory";
import { Role } from "@/features/roles/types";

export function buildSystemRoles(): Role[] {
  return [
    {
      id: null,
      systemRole: "BranchOwner",
      name: null,
      permissions: everyPermission.filter(
        (permission) =>
          ![
            "branchOwners.manage",
            "branches.create",
            "brandOwners.manage",
            "subscription.view",
          ].includes(permission),
      ),
      copiedFrom: null,
      memberCount: 1,
    },
    {
      id: null,
      systemRole: "Instructor",
      name: null,
      permissions: [...instructorPermissions],
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
    name: "Profe que cobra",
    permissions: [...instructorPermissions, "payments.view.all", "payments.record"],
    copiedFrom: "Instructor",
    memberCount: 0,
    ...overrides,
  };
}
