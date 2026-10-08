import { z } from "zod";
import { emailAddressSchema } from "@/forms/emailAddress";
import { authenticationLimits } from "@/features/authentication/authenticationLimits";
import { SignUpRequest } from "@/features/authentication/types";
import { CountryCode, getCountryPreset } from "@/features/business/countryPresets";
import {
  ageInYears,
  isValidBirthDate,
  parseBirthDate,
} from "@/features/students/birthDateFormatting";
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
  password: z
    .string()
    .min(
      authenticationLimits.passwordMinimumLength,
      translate("authentication.validation.passwordTooShort"),
    )
    .max(
      authenticationLimits.passwordMaximumLength,
      translate("authentication.validation.passwordTooLong"),
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
  countryCode: z.custom<CountryCode>((countryCode) => typeof countryCode === "string"),
  timeZoneId: z.string().min(1, translate("authentication.validation.timeZoneRequired")),
});

export type SignUpFormValues = z.input<typeof signUpSchema>;

export const signUpFieldNames = [
  "ownerFullName",
  "email",
  "password",
  "ownerBirthDate",
  "businessName",
  "timeZoneId",
] as const satisfies readonly (keyof SignUpFormValues)[];

export function toSignUpRequest(formValues: SignUpFormValues): SignUpRequest {
  const countryPreset = getCountryPreset(formValues.countryCode);
  return {
    ownerFullName: formValues.ownerFullName.trim(),
    email: formValues.email.trim(),
    password: formValues.password,
    businessName: formValues.businessName.trim(),
    timeZoneId: formValues.timeZoneId,
    currencyCode: countryPreset.currencyCode,
    defaultCountryCallingCode: countryPreset.callingCode,
    ownerBirthDate: parseBirthDate(formValues.ownerBirthDate) ?? "",
  };
}
