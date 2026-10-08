import { needsInstructor, Permission } from "@/features/members/permissions";
import { BusinessRole, SystemRole, systemRoles } from "@/features/members/types";
import { Role } from "@/features/roles/types";
import { translate, TranslationKey } from "@/i18n/translate";

export interface RoleChoice {
  key: string;
  role: BusinessRole;
  customRoleId: string | null;
  label: string;
  permissions: readonly Permission[] | null;
  needsInstructor: boolean;
}

const instructorRole: SystemRole = "Instructor";

export function systemRoleLabel(role: SystemRole): string {
  return translate(`roles.${role}` as TranslationKey);
}

export function isSystemRole(value: string): value is SystemRole {
  return (systemRoles as readonly string[]).includes(value);
}

export function roleKeyOf(role: BusinessRole, customRoleId: string | null): string {
  return role === "Custom" ? (customRoleId ?? "") : role;
}

export function roleKeyOfRole(role: Role): string {
  return role.id ?? role.systemRole ?? "";
}

export function roleName(role: Role): string {
  return role.systemRole ? systemRoleLabel(role.systemRole) : (role.name ?? "");
}

export function roleChoices(roles: Role[] | undefined): RoleChoice[] {
  if (roles === undefined) {
    return systemRoles.map((role) => ({
      key: role,
      role,
      customRoleId: null,
      label: systemRoleLabel(role),
      permissions: null,
      needsInstructor: role === instructorRole,
    }));
  }
  return roles.map((role) => ({
    key: roleKeyOfRole(role),
    role: role.systemRole ?? "Custom",
    customRoleId: role.id,
    label: roleName(role),
    permissions: role.permissions,
    needsInstructor: needsInstructor(role.permissions),
  }));
}

export function memberRoleLabel(
  role: BusinessRole,
  customRoleId: string | null,
  roles: Role[] | undefined,
): string {
  if (role !== "Custom") {
    return systemRoleLabel(role);
  }
  return (
    roles?.find((candidate) => candidate.id === customRoleId)?.name ?? translate("roles.Custom")
  );
}

export function findRoleChoice(roleKey: string, roles: Role[] | undefined): RoleChoice | undefined {
  return roleChoices(roles).find((choice) => choice.key === roleKey);
}

export function offeredRoleChoices(
  choices: RoleChoice[],
  memberPermissions: readonly Permission[],
  selectedRoleKey: string,
): RoleChoice[] {
  const canManageBranchOwners = memberPermissions.includes("branchOwners.manage");
  return choices.filter(
    (choice) =>
      choice.key === selectedRoleKey ||
      ((choice.role !== "BranchOwner" || canManageBranchOwners) &&
        (choice.permissions ?? []).every((permission) => memberPermissions.includes(permission))),
  );
}
