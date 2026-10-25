import { zodResolver } from "@hookform/resolvers/zod";
import { useLocalSearchParams, useRouter } from "expo-router";
import { useState } from "react";
import { Controller, useForm, useWatch } from "react-hook-form";
import { isApiError, isNetworkError } from "@/api/httpClient";
import { authenticationErrorCodes } from "@/features/authentication/authenticationErrorCodes";
import { authenticationLimits } from "@/features/authentication/authenticationLimits";
import { AuthenticationScreenLayout } from "@/features/authentication/components/AuthenticationScreenLayout";
import {
  CompleteSignUpFormValues,
  completeSignUpFieldNames,
  completeSignUpSchema,
  toCompleteSignUpRequest,
} from "@/features/authentication/completeSignUpSchema";
import { useSession } from "@/features/authentication/useSession";
import { applyServerFieldErrors } from "@/forms/applyServerFieldErrors";
import { useRequiredFieldsFilled } from "@/forms/requiredFields";
import { translate } from "@/i18n/translate";
import { routes } from "@/navigation/routes";
import { AppText } from "@/ui/AppText";
import { Banner } from "@/ui/Banner";
import { BrandMark } from "@/ui/BrandMark";
import { Button } from "@/ui/Button";
import { PasswordField } from "@/ui/PasswordField";
import { PasswordManagerUsername } from "@/ui/PasswordManagerUsername";

const badRequestStatus = 400;

type CompleteSignUpFailure = "invalidLink" | "emailTaken" | "network" | "unexpected";

export function CompleteSignUpScreen() {
  const router = useRouter();
  const { completeSignUp } = useSession();
  const { token, email } = useLocalSearchParams<{ token?: string; email?: string }>();
  const [failure, setFailure] = useState<CompleteSignUpFailure | null>(
    token ? null : "invalidLink",
  );
  const form = useForm<CompleteSignUpFormValues>({
    resolver: zodResolver(completeSignUpSchema),
    defaultValues: { password: "" },
    mode: "onTouched",
  });
  const areRequiredFieldsFilled = useRequiredFieldsFilled(form.control, ["password"]);
  const password = useWatch({ control: form.control, name: "password" });

  const handleCompleteError = (completeError: unknown) => {
    if (isNetworkError(completeError)) {
      setFailure("network");
      return;
    }
    if (
      isApiError(completeError) &&
      completeError.hasCode(authenticationErrorCodes.invalidSignUpToken)
    ) {
      setFailure("invalidLink");
      return;
    }
    if (isApiError(completeError) && completeError.hasCode(authenticationErrorCodes.emailTaken)) {
      setFailure("emailTaken");
      return;
    }
    if (
      isApiError(completeError) &&
      completeError.status === badRequestStatus &&
      applyServerFieldErrors(form, completeError.problem, completeSignUpFieldNames)
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
      await completeSignUp(toCompleteSignUpRequest(token, formValues));
      router.replace(routes.today);
    } catch (completeError) {
      handleCompleteError(completeError);
    }
  });

  const isFormShown = token && failure !== "invalidLink" && failure !== "emailTaken";

  return (
    <AuthenticationScreenLayout
      title={translate("authentication.completeSignUp.title")}
      onBack={() => router.replace(routes.welcome)}
      mark={<BrandMark />}
    >
      {failure === "invalidLink" ? (
        <Banner message={translate("authentication.completeSignUp.invalidLink")}>
          <Button
            variant="secondary"
            label={translate("authentication.completeSignUp.signUpAgain")}
            onPress={() => router.replace(routes.signUp)}
          />
        </Banner>
      ) : null}
      {failure === "emailTaken" ? (
        <Banner tone="warning" message={translate("authentication.signUp.emailTaken")}>
          <Button
            variant="secondary"
            label={translate("authentication.signUp.signInWithEmail")}
            onPress={() => router.replace(routes.signIn)}
          />
        </Banner>
      ) : null}
      {failure === "network" ? (
        <Banner message={translate("common.networkError")}>
          <Button variant="secondary" label={translate("common.retry")} onPress={submit} />
        </Banner>
      ) : null}
      {failure === "unexpected" ? <Banner message={translate("common.unexpectedError")} /> : null}
      {isFormShown ? (
        <>
          <AppText tone="muted">{translate("authentication.completeSignUp.hint")}</AppText>
          {email ? <PasswordManagerUsername email={email} /> : null}
          <Controller
            control={form.control}
            name="password"
            render={({ field, fieldState }) => (
              <PasswordField
                label={translate("authentication.completeSignUp.password")}
                hint={translate("authentication.signUp.passwordHint")}
                isHintSatisfied={password.length >= authenticationLimits.passwordMinimumLength}
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
            label={translate("authentication.completeSignUp.submit")}
            onPress={submit}
            disabled={!areRequiredFieldsFilled}
            isLoading={form.formState.isSubmitting}
          />
        </>
      ) : null}
    </AuthenticationScreenLayout>
  );
}
