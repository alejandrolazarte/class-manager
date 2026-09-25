import { z } from "zod";
import { isValidBirthDate, parseBirthDate } from "@/features/students/birthDateFormatting";
import { NewStudentRequest } from "@/features/students/types";
import { translate } from "@/i18n/translate";

export const studentLimits = {
  fullNameMinimumLength: 2,
  fullNameMaximumLength: 120,
  notesMaximumLength: 1000,
} as const;

export const studentFormSchema = z.object({
  fullName: z
    .string()
    .trim()
    .min(1, translate("students.validation.fullNameRequired"))
    .min(studentLimits.fullNameMinimumLength, translate("students.validation.fullNameTooShort"))
    .max(studentLimits.fullNameMaximumLength, translate("students.validation.fullNameTooLong")),
  birthDate: z
    .string()
    .trim()
    .refine(
      (birthDate) => birthDate.length === 0 || isValidBirthDate(birthDate),
      translate("students.validation.birthDateInvalid"),
    ),
  notes: z
    .string()
    .max(studentLimits.notesMaximumLength, translate("students.validation.notesTooLong")),
});

export type StudentFormValues = z.input<typeof studentFormSchema>;

export const emptyStudentFormValues: StudentFormValues = {
  fullName: "",
  birthDate: "",
  notes: "",
};

export function toNewStudentRequest(formValues: StudentFormValues): NewStudentRequest {
  const notes = formValues.notes.trim();
  return {
    fullName: formValues.fullName.trim(),
    birthDate: parseBirthDate(formValues.birthDate),
    notes: notes.length > 0 ? notes : null,
  };
}

export function normalizeStudentName(fullName: string): string {
  return fullName.trim().toLocaleLowerCase();
}
