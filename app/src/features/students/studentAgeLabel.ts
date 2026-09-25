import { ageInYears } from "@/features/students/birthDateFormatting";
import { translate } from "@/i18n/translate";

const singleYear = 1;

export function studentAgeLabel(birthDate: string | null): string | null {
  if (birthDate === null) {
    return null;
  }
  const age = ageInYears(birthDate);
  return age === singleYear ? translate("students.ageOneYear") : translate("students.age", { age });
}
