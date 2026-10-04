import { zodResolver } from "@hookform/resolvers/zod";
import { useLocalSearchParams, useRouter } from "expo-router";
import { useState } from "react";
import { Controller, useForm, useWatch } from "react-hook-form";
import { isApiError, isNetworkError } from "@/api/httpClient";
import { resetPassword } from "@/features/authentication/authenticationApi";
import { authenticationErrorCodes } from "@/features/authentication/authenticationErrorCodes";
import { AuthenticationScreenLayout } from "@/features/authentication/components/AuthenticationScreenLayout";
import {
  ResetPasswordFormValues,
  resetPasswordFieldNames,
  resetPasswordSchema,
  toResetPasswordRequest,
} from "@/features/authentication/resetPasswordSchema";
import { applyServerFieldErrors } from "@/forms/applyServerFieldErrors";
import { translate } from "@/i18n/translate";
import { routes } from "@/navigation/routes";
import { Banner } from "@/ui/Banner";
import { BrandMark } from "@/ui/BrandMark";
import { Button } from "@/ui/Button";
import { PasswordField } from "@/ui/PasswordField";
import { useToast } from "@/ui/ToastProvider";
import { useRequiredFieldsFilled } from "@/forms/requiredFields";
import { authenticationLimits } from "@/features/authentication/authenticationLimits";

const badRequestStatus = 400;

type ResetPasswordFailure = "invalidLink" | "network" | "unexpected";

export function ResetPasswordScreen() {
  const router = useRouter();
  const { showToast } = useToast();
  const { token } = useLocalSearchParams<{ token?: string }>();
  const [failure, setFailure] = useState<ResetPasswordFailure | null>(token ? null : "invalidLink");
  const form = useForm<ResetPasswordFormValues>({
    resolver: zodResolver(resetPasswordSchema),
    defaultValues: { newPassword: "" },
    mode: "onTouched",
  });
  const areRequiredFieldsFilled = useRequiredFieldsFilled(form.control, ["newPassword"]);
  const newPassword = useWatch({ control: form.control, name: "newPassword" });

  const handleResetError = (resetError: unknown) => {
    if (isNetworkError(resetError)) {
      setFailure("network");
      return;
    }
    if (
      isApiError(resetError) &&
      resetError.hasCode(authenticationErrorCodes.invalidPasswordResetToken)
    ) {
      setFailure("invalidLink");
      return;
    }
    if (
      isApiError(resetError) &&
      resetError.status === badRequestStatus &&
      applyServerFieldErrors(form, resetError.problem, resetPasswordFieldNames)
    ) {
      return;
    }
    setFailure("unexpected");
  };

  const submit = form.handleSubmit(async (formValues) => {
    if (!token) {
      return;
    }
    setFailure(null);
    try {
      await resetPassword(toResetPasswordRequest(token, formValues));
      showToast(translate("authentication.resetPassword.saved"));
      router.replace(routes.signIn);
    } catch (resetError) {
      handleResetError(resetError);
    }
  });

  const goToSignIn = () => router.replace(routes.signIn);

  return (
    <AuthenticationScreenLayout
      title={translate("authentication.resetPassword.title")}
      onBack={goToSignIn}
      mark={<BrandMark />}
    >
      {failure === "invalidLink" ? (
        <Banner message={translate("authentication.resetPassword.invalidLink")}>
          <Button
            variant="secondary"
            label={translate("authentication.resetPassword.askForNewLink")}
            onPress={() => router.replace(routes.forgotPassword(""))}
          />
        </Banner>
      ) : null}
      {failure === "network" ? (
        <Banner message={translate("common.networkError")}>
          <Button variant="secondary" label={translate("common.retry")} onPress={submit} />
        </Banner>
      ) : null}
      {failure === "unexpected" ? <Banner message={translate("common.unexpectedError")} /> : null}
      {token && failure !== "invalidLink" ? (
        <>
          <Controller
            control={form.control}
            name="newPassword"
            render={({ field, fieldState }) => (
              <PasswordField
                label={translate("authentication.resetPassword.newPassword")}
                hint={translate("authentication.signUp.passwordHint")}
                isHintSatisfied={newPassword.length >= authenticationLimits.passwordMinimumLength}
                autoComplete="new-password"
                value={field.value}
                onChangeText={field.onChange}
                onBlur={field.onBlur}
                onSubmitEditing={submit}
                errorMessage={fieldState.error?.message}
              />
            )}
          />
          <Button
            label={translate("authentication.resetPassword.submit")}
            onPress={submit}
            disabled={!areRequiredFieldsFilled}
            isLoading={form.formState.isSubmitting}
          />
        </>
      ) : null}
    </AuthenticationScreenLayout>
  );
}
