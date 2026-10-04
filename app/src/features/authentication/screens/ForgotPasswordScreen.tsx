import { zodResolver } from "@hookform/resolvers/zod";
import { useLocalSearchParams, useRouter } from "expo-router";
import { useState } from "react";
import { Controller, useForm } from "react-hook-form";
import { isNetworkError } from "@/api/httpClient";
import { requestPasswordReset } from "@/features/authentication/authenticationApi";
import { AuthenticationScreenLayout } from "@/features/authentication/components/AuthenticationScreenLayout";
import {
  ForgotPasswordFormValues,
  forgotPasswordSchema,
  toPasswordResetRequest,
} from "@/features/authentication/forgotPasswordSchema";
import { translate } from "@/i18n/translate";
import { routes } from "@/navigation/routes";
import { AppText } from "@/ui/AppText";
import { Banner } from "@/ui/Banner";
import { BrandMark } from "@/ui/BrandMark";
import { Button } from "@/ui/Button";
import { TextField } from "@/ui/TextField";
import { RequiredFieldsLegend } from "@/ui/RequiredFieldsLegend";
import { useRequiredFieldsFilled } from "@/forms/requiredFields";

type ForgotPasswordFailure = "network" | "unexpected";

export function ForgotPasswordScreen() {
  const router = useRouter();
  const { email: prefilledEmail } = useLocalSearchParams<{ email?: string }>();
  const [sentToEmail, setSentToEmail] = useState<string | null>(null);
  const [failure, setFailure] = useState<ForgotPasswordFailure | null>(null);
  const form = useForm<ForgotPasswordFormValues>({
    resolver: zodResolver(forgotPasswordSchema),
    defaultValues: { email: prefilledEmail ?? "" },
    mode: "onTouched",
  });
  const areRequiredFieldsFilled = useRequiredFieldsFilled(form.control, ["email"]);

  const submit = form.handleSubmit(async (formValues) => {
    setFailure(null);
    const request = toPasswordResetRequest(formValues);
    try {
      await requestPasswordReset(request);
      setSentToEmail(request.email);
    } catch (requestError) {
      setFailure(isNetworkError(requestError) ? "network" : "unexpected");
    }
  });

  const goToSignIn = () => router.replace(routes.signIn);

  return (
    <AuthenticationScreenLayout
      title={translate("authentication.forgotPassword.title")}
      onBack={goToSignIn}
      mark={<BrandMark />}
    >
      {sentToEmail ? (
        <>
          <Banner
            tone="info"
            icon="password"
            message={translate("authentication.forgotPassword.sent", { email: sentToEmail })}
          />
          <Button
            variant="secondary"
            label={translate("authentication.forgotPassword.backToSignIn")}
            onPress={goToSignIn}
          />
        </>
      ) : (
        <>
          <AppText tone="muted">{translate("authentication.forgotPassword.hint")}</AppText>
          {failure === "network" ? (
            <Banner message={translate("common.networkError")}>
              <Button variant="secondary" label={translate("common.retry")} onPress={submit} />
            </Banner>
          ) : null}
          {failure === "unexpected" ? (
            <Banner message={translate("common.unexpectedError")} />
          ) : null}
          <RequiredFieldsLegend />
          <Controller
            control={form.control}
            name="email"
            render={({ field, fieldState }) => (
              <TextField
                label={translate("authentication.forgotPassword.email")}
                isRequired
                keyboardType="email-address"
                autoCapitalize="none"
                autoComplete="email"
                value={field.value}
                onChangeText={field.onChange}
                onBlur={field.onBlur}
                onSubmitEditing={submit}
                errorMessage={fieldState.error?.message}
              />
            )}
          />
          <Button
            label={translate("authentication.forgotPassword.submit")}
            onPress={submit}
            disabled={!areRequiredFieldsFilled}
            isLoading={form.formState.isSubmitting}
          />
        </>
      )}
    </AuthenticationScreenLayout>
  );
}
