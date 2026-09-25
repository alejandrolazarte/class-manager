import { z } from "zod";
import { authenticationLimits } from "@/features/authentication/authenticationLimits";
import { SignInRequest } from "@/features/authentication/types";
import { translate } from "@/i18n/translate";

export const signInSchema = z.object({
  email: z
    .email(translate("authentication.validation.emailInvalid"))
    .max(
      authenticationLimits.emailMaximumLength,
      translate("authentication.validation.emailInvalid"),
    ),
  password: z.string().min(1, translate("authentication.validation.passwordRequired")),
});

export type SignInFormValues = z.input<typeof signInSchema>;

export const signInFieldNames = [
  "email",
  "password",
] as const satisfies readonly (keyof SignInFormValues)[];

export function toSignInRequest(formValues: SignInFormValues): SignInRequest {
  return { email: formValues.email.trim(), password: formValues.password };
}
