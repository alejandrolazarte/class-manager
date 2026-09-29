import { BusinessRole } from "@/features/members/types";
import { memberRoleLabel } from "@/features/roles/roleChoices";
import { Role } from "@/features/roles/types";
import { translate } from "@/i18n/translate";

export function roleDescription(
  role: BusinessRole,
  customRoleId: string | null,
  roles: Role[] | undefined,
  coachFullName: string | undefined,
): string {
  const label = memberRoleLabel(role, customRoleId, roles);
  return coachFullName
    ? translate("roles.withCoach", { role: label, coach: coachFullName })
    : label;
}
