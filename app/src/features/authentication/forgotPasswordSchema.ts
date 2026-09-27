import { z } from "zod";
import { authenticationLimits } from "@/features/authentication/authenticationLimits";
import { PasswordResetRequest } from "@/features/authentication/types";
import { translate } from "@/i18n/translate";

export const forgotPasswordSchema = z.object({
  email: z
    .email(translate("authentication.validation.emailInvalid"))
    .max(
      authenticationLimits.emailMaximumLength,
      translate("authentication.validation.emailInvalid"),
    ),
});

export type ForgotPasswordFormValues = z.input<typeof forgotPasswordSchema>;

export function toPasswordResetRequest(formValues: ForgotPasswordFormValues): PasswordResetRequest {
  return { email: formValues.email.trim() };
}
