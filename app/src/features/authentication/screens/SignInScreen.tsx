import { zodResolver } from "@hookform/resolvers/zod";
import { useLocalSearchParams, useRouter } from "expo-router";
import { useState } from "react";
import { Controller, useForm } from "react-hook-form";
import { Linking, Pressable } from "react-native";
import { isApiError, isNetworkError } from "@/api/httpClient";
import { appConfiguration } from "@/config/appConfiguration";
import { authenticationErrorCodes } from "@/features/authentication/authenticationErrorCodes";
import { AuthenticationScreenLayout } from "@/features/authentication/components/AuthenticationScreenLayout";
import {
  SignInFormValues,
  signInFieldNames,
  signInSchema,
  toSignInRequest,
} from "@/features/authentication/signInSchema";
import { useSession } from "@/features/authentication/useSession";
import { applyServerFieldErrors } from "@/forms/applyServerFieldErrors";
import { translate } from "@/i18n/translate";
import { routes } from "@/navigation/routes";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";
import { PasswordField } from "@/ui/PasswordField";
import { TextField } from "@/ui/TextField";
import { AppText } from "@/ui/AppText";
import { BrandMark } from "@/ui/BrandMark";

const badRequestStatus = 400;
const whatsAppBaseUrl = "https://wa.me/";

type SignInFailure = "invalidCredentials" | "lockedOut" | "network" | "unexpected";

export function SignInScreen() {
  const router = useRouter();
  const { email: prefilledEmail } = useLocalSearchParams<{ email?: string }>();
  const { signIn } = useSession();
  const [signInFailure, setSignInFailure] = useState<SignInFailure | null>(null);
  const [isForgotPasswordHelpVisible, setIsForgotPasswordHelpVisible] = useState(false);
  const supportWhatsAppNumber = appConfiguration.supportWhatsAppNumber;
  const form = useForm<SignInFormValues>({
    resolver: zodResolver(signInSchema),
    defaultValues: { email: prefilledEmail ?? "", password: "" },
    mode: "onTouched",
  });

  const handleSignInError = (signInError: unknown) => {
    if (isNetworkError(signInError)) {
      setSignInFailure("network");
      return;
    }
    if (
      isApiError(signInError) &&
      signInError.hasCode(authenticationErrorCodes.invalidCredentials)
    ) {
      setSignInFailure("invalidCredentials");
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
      handleSignInError(signInError);
    }
  });

  const askForResetLink = () => {
    const message = translate("authentication.signIn.resetLinkMessage", {
      email: form.getValues("email").trim(),
    });
    Linking.openURL(
      `${whatsAppBaseUrl}${supportWhatsAppNumber}?text=${encodeURIComponent(message)}`,
    );
  };

  return (
    <AuthenticationScreenLayout
      title={translate("authentication.signIn.title")}
      onBack={() => router.replace(routes.welcome)}
      mark={<BrandMark />}
    >
      {signInFailure === "invalidCredentials" ? (
        <Banner message={translate("authentication.signIn.invalidCredentials")} />
      ) : null}
      {signInFailure === "lockedOut" ? (
        <Banner tone="warning" message={translate("authentication.signIn.lockedOut")} />
      ) : null}
      {signInFailure === "network" ? (
        <Banner message={translate("common.networkError")}>
          <Button variant="secondary" label={translate("common.retry")} onPress={submit} />
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
        isLoading={form.formState.isSubmitting}
      />
      <Pressable
        accessibilityRole="button"
        className="py-1.5"
        onPress={() => setIsForgotPasswordHelpVisible(true)}
      >
        <AppText tone="primary" variant="link" className="text-center">
          {translate("authentication.signIn.forgotPassword")}
        </AppText>
      </Pressable>
      {isForgotPasswordHelpVisible ? (
        <Banner
          tone="info"
          icon="password"
          message={translate("authentication.signIn.forgotPasswordHelp")}
        >
          {supportWhatsAppNumber ? (
            <Button
              variant="secondary"
              label={translate("authentication.signIn.askForResetLink")}
              onPress={askForResetLink}
            />
          ) : null}
        </Banner>
      ) : null}
      <Pressable
        accessibilityRole="link"
        className="py-1.5"
        onPress={() => router.replace(routes.signUp)}
      >
        <AppText tone="primary" variant="link" className="text-center">
          {translate("authentication.signIn.goToSignUp")}
        </AppText>
      </Pressable>
    </AuthenticationScreenLayout>
  );
}
