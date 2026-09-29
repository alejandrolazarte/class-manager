import { BusinessRole } from "@/features/members/types";
import { translate, TranslationKey } from "@/i18n/translate";

export function roleLabel(role: BusinessRole): string {
  return translate(`roles.${role}` as TranslationKey);
}

export function roleDescription(role: BusinessRole, coachFullName: string | undefined): string {
  return role === "Coach" && coachFullName
    ? translate("roles.coachOf", { coach: coachFullName })
    : roleLabel(role);
}
