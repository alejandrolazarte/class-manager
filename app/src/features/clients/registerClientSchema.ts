import { z } from "zod";
import { countDigits } from "@/features/clients/phoneNumberFormatting";
import { RegisterClientRequest } from "@/features/clients/types";
import { translate } from "@/i18n/translate";

export const clientLimits = {
  fullNameMinimumLength: 2,
  fullNameMaximumLength: 120,
  phoneNumberMinimumDigits: 8,
  phoneNumberMaximumDigits: 15,
  emailMaximumLength: 254,
  notesMaximumLength: 1000,
} as const;

const phoneNumberAllowedCharactersPattern = /^[+\d\s()-]+$/;

export const registerClientSchema = z.object({
  fullName: z
    .string()
    .trim()
    .min(1, translate("clients.validation.fullNameRequired"))
    .min(clientLimits.fullNameMinimumLength, translate("clients.validation.fullNameTooShort"))
    .max(clientLimits.fullNameMaximumLength, translate("clients.validation.fullNameTooLong")),
  phoneNumber: z
    .string()
    .trim()
    .min(1, translate("clients.validation.phoneNumberRequired"))
    .regex(phoneNumberAllowedCharactersPattern, translate("clients.validation.phoneNumberInvalid"))
    .refine(
      (phoneNumber) =>
        countDigits(phoneNumber) >= clientLimits.phoneNumberMinimumDigits &&
        countDigits(phoneNumber) <= clientLimits.phoneNumberMaximumDigits,
      translate("clients.validation.phoneNumberInvalid"),
    ),
  email: z.union([
    z.literal(""),
    z
      .email(translate("clients.validation.emailInvalid"))
      .max(clientLimits.emailMaximumLength, translate("clients.validation.emailInvalid")),
  ]),
  notes: z
    .string()
    .max(clientLimits.notesMaximumLength, translate("clients.validation.notesTooLong")),
});

export type RegisterClientFormValues = z.input<typeof registerClientSchema>;

export const emptyRegisterClientFormValues: RegisterClientFormValues = {
  fullName: "",
  phoneNumber: "",
  email: "",
  notes: "",
};

export function toRegisterClientRequest(
  formValues: RegisterClientFormValues,
): RegisterClientRequest {
  const email = formValues.email.trim();
  const notes = formValues.notes.trim();
  return {
    fullName: formValues.fullName.trim(),
    phoneNumber: formValues.phoneNumber.trim(),
    ...(email.length > 0 ? { email } : {}),
    ...(notes.length > 0 ? { notes } : {}),
  };
}
