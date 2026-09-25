import { z } from "zod";
import { authenticationLimits } from "@/features/authentication/authenticationLimits";
import { translate } from "@/i18n/translate";

export const businessSettingsSchema = z.object({
  name: z
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
});

export type BusinessSettingsFormValues = z.input<typeof businessSettingsSchema>;

export const businessSettingsFieldNames = [
  "name",
] as const satisfies readonly (keyof BusinessSettingsFormValues)[];
