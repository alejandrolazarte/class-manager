import { z } from "zod";
import { authenticationLimits } from "@/features/authentication/authenticationLimits";
import { ResetPasswordRequest } from "@/features/authentication/types";
import { translate } from "@/i18n/translate";

export const resetPasswordSchema = z.object({
  newPassword: z
    .string()
    .min(
      authenticationLimits.passwordMinimumLength,
      translate("authentication.validation.passwordTooShort"),
    )
    .max(
      authenticationLimits.passwordMaximumLength,
      translate("authentication.validation.passwordTooLong"),
    ),
});

export type ResetPasswordFormValues = z.input<typeof resetPasswordSchema>;

export const resetPasswordFieldNames = [
  "newPassword",
] as const satisfies readonly (keyof ResetPasswordFormValues)[];

export function toResetPasswordRequest(
  token: string,
  formValues: ResetPasswordFormValues,
): ResetPasswordRequest {
  return { token, newPassword: formValues.newPassword };
}
