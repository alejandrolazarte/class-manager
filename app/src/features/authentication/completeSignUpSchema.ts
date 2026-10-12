import { z } from "zod";
import { authenticationLimits } from "@/features/authentication/authenticationLimits";
import { CompleteSignUpRequest } from "@/features/authentication/types";
import { translate } from "@/i18n/translate";

export const completeSignUpSchema = z.object({
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
});

export type CompleteSignUpFormValues = z.input<typeof completeSignUpSchema>;

export const completeSignUpFieldNames = [
  "password",
] as const satisfies readonly (keyof CompleteSignUpFormValues)[];

export function toCompleteSignUpRequest(
  token: string,
  formValues: CompleteSignUpFormValues,
): CompleteSignUpRequest {
  return { token, password: formValues.password };
}
