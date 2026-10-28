import { isAdult } from "@/dates/birthDateFormatting";
import { translate } from "@/i18n/translate";

export function responsibleLabel(clientFullName: string, birthDate: string | null): string {
  return birthDate !== null && isAdult(birthDate)
    ? translate("students.list.comesWith", { name: clientFullName })
    : translate("students.list.responsible", { name: clientFullName });
}
