import { z } from "zod";
import { Instructor, SaveInstructorRequest } from "@/features/instructors/types";
import { translate } from "@/i18n/translate";

export const instructorLimits = {
  fullNameMinimumLength: 2,
  fullNameMaximumLength: 120,
} as const;

export const instructorSchema = z.object({
  fullName: z
    .string()
    .trim()
    .min(1, translate("instructors.validation.fullNameRequired"))
    .min(
      instructorLimits.fullNameMinimumLength,
      translate("instructors.validation.fullNameTooShort"),
    )
    .max(
      instructorLimits.fullNameMaximumLength,
      translate("instructors.validation.fullNameTooLong"),
    ),
});

export type InstructorFormValues = z.input<typeof instructorSchema>;

export const instructorFieldNames = [
  "fullName",
] as const satisfies readonly (keyof InstructorFormValues)[];

export function toInstructorFormValues(instructor?: Instructor): InstructorFormValues {
  return { fullName: instructor?.fullName ?? "" };
}

export function toSaveInstructorRequest(formValues: InstructorFormValues): SaveInstructorRequest {
  return { fullName: formValues.fullName.trim() };
}
