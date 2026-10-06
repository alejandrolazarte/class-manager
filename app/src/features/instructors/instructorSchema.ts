import { z } from "zod";
import { emailAddressSchema } from "@/forms/emailAddress";
import { Instructor, SaveInstructorRequest } from "@/features/instructors/types";
import { translate } from "@/i18n/translate";

export const instructorLimits = {
  fullNameMinimumLength: 2,
  fullNameMaximumLength: 120,
  emailMaximumLength: 254,
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
  email: z
    .string()
    .trim()
    .pipe(
      z.union([
        z.literal(""),
        emailAddressSchema(translate("instructors.validation.emailInvalid")).max(
          instructorLimits.emailMaximumLength,
          translate("instructors.validation.emailInvalid"),
        ),
      ]),
    ),
});

export type InstructorFormValues = z.input<typeof instructorSchema>;

export const instructorFieldNames = [
  "fullName",
  "email",
] as const satisfies readonly (keyof InstructorFormValues)[];

export function toInstructorFormValues(instructor?: Instructor): InstructorFormValues {
  return { fullName: instructor?.fullName ?? "", email: instructor?.email ?? "" };
}

export function toSaveInstructorRequest(formValues: InstructorFormValues): SaveInstructorRequest {
  const trimmedEmail = formValues.email.trim();
  return {
    fullName: formValues.fullName.trim(),
    email: trimmedEmail.length === 0 ? null : trimmedEmail,
  };
}
