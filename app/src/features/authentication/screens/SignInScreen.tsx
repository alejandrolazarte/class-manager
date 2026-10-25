import { zodResolver } from "@hookform/resolvers/zod";
import { useLocalSearchParams, useRouter } from "expo-router";
import { useState } from "react";
import { Controller, useForm } from "react-hook-form";
import { isApiError, isNetworkError } from "@/api/httpClient";
import { authenticationErrorCodes } from "@/features/authentication/authenticationErrorCodes";
import { AuthenticationScreenLayout } from "@/features/authentication/components/AuthenticationScreenLayout";
import {
  SignInFormValues,
  signInFieldNames,
  signInSchema,
  toSignInRequest,
} from "@/features/authentication/signInSchema";
import { useSession } from "@/features/authentication/useSession";
import { isPasskeyPromptCancelled } from "@/features/passkeys/passkeyErrors";
import { usePasskeySupport } from "@/features/passkeys/usePasskeySupport";
import { applyServerFieldErrors } from "@/forms/applyServerFieldErrors";
import { translate } from "@/i18n/translate";
import { routes } from "@/navigation/routes";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";
import { TextLink } from "@/ui/TextLink";
import { PasswordField } from "@/ui/PasswordField";
import { TextField } from "@/ui/TextField";
import { BrandMark } from "@/ui/BrandMark";
import { useRequiredFieldsFilled } from "@/forms/requiredFields";

const badRequestStatus = 400;

type SignInFailure =
  "invalidCredentials" | "passkeyNotVerified" | "lockedOut" | "network" | "unexpected";

type SignInMethod = "password" | "passkey";

export function SignInScreen() {
  const router = useRouter();
  const { email: prefilledEmail } = useLocalSearchParams<{ email?: string }>();
  const { signIn, signInWithPasskey } = useSession();
  const isPasskeySupported = usePasskeySupport();
  const [signInFailure, setSignInFailure] = useState<SignInFailure | null>(null);
  const [failedMethod, setFailedMethod] = useState<SignInMethod>("password");
  const [isSigningInWithPasskey, setIsSigningInWithPasskey] = useState(false);
  const form = useForm<SignInFormValues>({
    resolver: zodResolver(signInSchema),
    defaultValues: { email: prefilledEmail ?? "", password: "" },
    mode: "onTouched",
  });
  const areRequiredFieldsFilled = useRequiredFieldsFilled(form.control, signInFieldNames);

  const handleSignInError = (signInError: unknown, method: SignInMethod) => {
    setFailedMethod(method);
    if (isNetworkError(signInError)) {
      setSignInFailure("network");
      return;
    }
    if (
      isApiError(signInError) &&
      signInError.hasCode(authenticationErrorCodes.invalidCredentials)
    ) {
      setSignInFailure(method === "passkey" ? "passkeyNotVerified" : "invalidCredentials");
      return;
    }
    if (isApiError(signInError) && signInError.hasCode(authenticationErrorCodes.lockedOut)) {
      setSignInFailure("lockedOut");
      return;
    }
    if (
      isApiError(signInError) &&
      signInError.status === badRequestStatus &&
      applyServerFieldErrors(form, signInError.problem, signInFieldNames)
    ) {
      return;
    }
    setSignInFailure("unexpected");
  };

  const submit = form.handleSubmit(async (formValues) => {
    setSignInFailure(null);
    try {
      await signIn(toSignInRequest(formValues));
    } catch (signInError) {
      handleSignInError(signInError, "password");
    }
  });

  const signInUsingPasskey = async () => {
    setSignInFailure(null);
    setIsSigningInWithPasskey(true);
    try {
      await signInWithPasskey();
    } catch (signInError) {
      if (!isPasskeyPromptCancelled(signInError)) {
        handleSignInError(signInError, "passkey");
      }
    } finally {
      setIsSigningInWithPasskey(false);
    }
  };

  return (
    <AuthenticationScreenLayout
      title={translate("authentication.signIn.title")}
      onBack={() => router.replace(routes.welcome)}
      mark={<BrandMark />}
      footer={
        <TextLink
          accessibilityRole="link"
          alignment="center"
          label={translate("authentication.signIn.goToSignUp")}
          onPress={() => router.replace(routes.signUp)}
        />
      }
    >
      {signInFailure === "invalidCredentials" ? (
        <Banner message={translate("authentication.signIn.invalidCredentials")} />
      ) : null}
      {signInFailure === "passkeyNotVerified" ? (
        <Banner message={translate("authentication.signIn.passkeyNotVerified")} />
      ) : null}
      {signInFailure === "lockedOut" ? (
        <Banner tone="warning" message={translate("authentication.signIn.lockedOut")} />
      ) : null}
      {signInFailure === "network" ? (
        <Banner message={translate("common.networkError")}>
          <Button
            variant="secondary"
            label={translate("common.retry")}
            onPress={failedMethod === "passkey" ? signInUsingPasskey : submit}
          />
        </Banner>
      ) : null}
      {signInFailure === "unexpected" ? (
        <Banner message={translate("common.unexpectedError")} />
      ) : null}
      <Controller
        control={form.control}
        name="email"
        render={({ field, fieldState }) => (
          <TextField
            label={translate("authentication.signIn.email")}
            keyboardType="email-address"
            autoCapitalize="none"
            autoComplete="email"
            value={field.value}
            onChangeText={field.onChange}
            onBlur={field.onBlur}
            errorMessage={fieldState.error?.message}
          />
        )}
      />
      <Controller
        control={form.control}
        name="password"
        render={({ field, fieldState }) => (
          <PasswordField
            label={translate("authentication.signIn.password")}
            autoComplete="current-password"
            value={field.value}
            onChangeText={field.onChange}
            onBlur={field.onBlur}
            onSubmitEditing={submit}
            errorMessage={fieldState.error?.message}
          />
        )}
      />
      <Button
        label={translate("authentication.signIn.submit")}
        onPress={submit}
        disabled={!areRequiredFieldsFilled}
        isLoading={form.formState.isSubmitting}
      />
      {isPasskeySupported ? (
        <Button
          variant="secondary"
          icon="passkey"
          label={translate("authentication.signIn.withPasskey")}
          onPress={signInUsingPasskey}
          isLoading={isSigningInWithPasskey}
        />
      ) : null}
      <TextLink
        alignment="center"
        label={translate("authentication.signIn.forgotPassword")}
        onPress={() => router.push(routes.forgotPassword(form.getValues("email").trim()))}
      />
    </AuthenticationScreenLayout>
  );
}
