import { z } from "zod";
import { emailAddressSchema } from "@/forms/emailAddress";
import { authenticationLimits } from "@/features/authentication/authenticationLimits";
import { SignUpRequest } from "@/features/authentication/types";
import { ageInYears, isValidBirthDate, parseBirthDate } from "@/dates/birthDateFormatting";
import { translate } from "@/i18n/translate";

const adultAge = 18;

export const signUpSchema = z.object({
  ownerFullName: z
    .string()
    .trim()
    .min(1, translate("authentication.validation.fullNameRequired"))
    .min(
      authenticationLimits.fullNameMinimumLength,
      translate("authentication.validation.fullNameTooShort"),
    )
    .max(
      authenticationLimits.fullNameMaximumLength,
      translate("authentication.validation.fullNameTooLong"),
    ),
  email: emailAddressSchema(translate("authentication.validation.emailInvalid")).max(
    authenticationLimits.emailMaximumLength,
    translate("authentication.validation.emailInvalid"),
  ),
  ownerBirthDate: z
    .string()
    .trim()
    .refine(isValidBirthDate, translate("birthDate.invalid"))
    .refine((typedBirthDate) => {
      const isoBirthDate = parseBirthDate(typedBirthDate);
      return isoBirthDate === null || ageInYears(isoBirthDate) >= adultAge;
    }, translate("authentication.validation.ownerMustBeAdult")),
  businessName: z
    .string()
    .trim()
    .min(1, translate("authentication.validation.businessNameRequired"))
    .min(
      authenticationLimits.businessNameMinimumLength,
      translate("authentication.validation.businessNameTooShort"),
    )
    .max(
      authenticationLimits.businessNameMaximumLength,
      translate("authentication.validation.businessNameTooLong"),
    ),
  countryCode: z.string(),
  timeZoneId: z.string().min(1, translate("authentication.validation.timeZoneRequired")),
  acceptsLegalTerms: z.boolean().refine((isAccepted) => isAccepted),
});

export type SignUpFormValues = z.input<typeof signUpSchema>;

export const signUpFieldNames = [
  "ownerFullName",
  "email",
  "ownerBirthDate",
  "businessName",
  "timeZoneId",
] as const satisfies readonly (keyof SignUpFormValues)[];

export function toSignUpRequest(formValues: SignUpFormValues, currencyCode: string): SignUpRequest {
  return {
    ownerFullName: formValues.ownerFullName.trim(),
    email: formValues.email.trim(),
    businessName: formValues.businessName.trim(),
    countryCode: formValues.countryCode,
    timeZoneId: formValues.timeZoneId,
    currencyCode,
    ownerBirthDate: parseBirthDate(formValues.ownerBirthDate) ?? "",
    acceptsLegalTerms: formValues.acceptsLegalTerms,
  };
}
