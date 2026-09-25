import { zodResolver } from "@hookform/resolvers/zod";
import { useRouter } from "expo-router";
import { useState } from "react";
import { Controller, useForm, useWatch } from "react-hook-form";
import { Pressable, Text, View } from "react-native";
import { isApiError, isNetworkError } from "@/api/httpClient";
import { authenticationErrorCodes } from "@/features/authentication/authenticationErrorCodes";
import { AuthenticationScreenLayout } from "@/features/authentication/components/AuthenticationScreenLayout";
import {
  SignUpFormValues,
  signUpFieldNames,
  signUpSchema,
  toSignUpRequest,
} from "@/features/authentication/signUpSchema";
import { useSession } from "@/features/authentication/useSession";
import { CountryPicker } from "@/features/business/components/CountryPicker";
import {
  getCountrySelectionForTimeZone,
  getDeviceTimeZone,
} from "@/features/business/countryPresets";
import { applyServerFieldErrors } from "@/forms/applyServerFieldErrors";
import { translate } from "@/i18n/translate";
import { routes } from "@/navigation/routes";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";
import { PasswordField } from "@/ui/PasswordField";
import { TextField } from "@/ui/TextField";

const badRequestStatus = 400;

type SignUpFailure =
  { kind: "emailTaken"; email: string } | { kind: "network" } | { kind: "unexpected" };

function SectionTitle({ title }: { title: string }) {
  return (
    <Text accessibilityRole="header" className="text-lg font-semibold text-gray-900">
      {title}
    </Text>
  );
}

export function SignUpScreen() {
  const router = useRouter();
  const { signUp } = useSession();
  const [signUpFailure, setSignUpFailure] = useState<SignUpFailure | null>(null);
  const [initialCountrySelection] = useState(() =>
    getCountrySelectionForTimeZone(getDeviceTimeZone()),
  );
  const form = useForm<SignUpFormValues>({
    resolver: zodResolver(signUpSchema),
    defaultValues: {
      ownerFullName: "",
      email: "",
      password: "",
      businessName: "",
      ...initialCountrySelection,
    },
    mode: "onTouched",
  });
  const [countryCode, timeZoneId] = useWatch({
    control: form.control,
    name: ["countryCode", "timeZoneId"],
  });

  const handleSignUpError = (signUpError: unknown, email: string) => {
    if (isNetworkError(signUpError)) {
      setSignUpFailure({ kind: "network" });
      return;
    }
    if (isApiError(signUpError) && signUpError.hasCode(authenticationErrorCodes.emailTaken)) {
      setSignUpFailure({ kind: "emailTaken", email });
      return;
    }
    if (
      isApiError(signUpError) &&
      signUpError.status === badRequestStatus &&
      applyServerFieldErrors(form, signUpError.problem, signUpFieldNames)
    ) {
      return;
    }
    setSignUpFailure({ kind: "unexpected" });
  };

  const submit = form.handleSubmit(async (formValues) => {
    setSignUpFailure(null);
    const signUpRequest = toSignUpRequest(formValues);
    try {
      await signUp(signUpRequest);
    } catch (signUpError) {
      handleSignUpError(signUpError, signUpRequest.email);
    }
  });

  return (
    <AuthenticationScreenLayout title={translate("authentication.signUp.title")}>
      {signUpFailure?.kind === "emailTaken" ? (
        <Banner tone="warning" message={translate("authentication.signUp.emailTaken")}>
          <Button
            variant="secondary"
            label={translate("authentication.signUp.signInWithEmail")}
            onPress={() =>
              router.replace({ pathname: routes.signIn, params: { email: signUpFailure.email } })
            }
          />
        </Banner>
      ) : null}
      {signUpFailure?.kind === "network" ? (
        <Banner message={translate("common.networkError")}>
          <Button variant="secondary" label={translate("common.retry")} onPress={submit} />
        </Banner>
      ) : null}
      {signUpFailure?.kind === "unexpected" ? (
        <Banner message={translate("common.unexpectedError")} />
      ) : null}
      <View className="gap-4">
        <SectionTitle title={translate("authentication.signUp.accountSection")} />
        <Controller
          control={form.control}
          name="ownerFullName"
          render={({ field, fieldState }) => (
            <TextField
              label={translate("authentication.signUp.ownerFullName")}
              autoCapitalize="words"
              autoComplete="name"
              value={field.value}
              onChangeText={field.onChange}
              onBlur={field.onBlur}
              errorMessage={fieldState.error?.message}
            />
          )}
        />
        <Controller
          control={form.control}
          name="email"
          render={({ field, fieldState }) => (
            <TextField
              label={translate("authentication.signUp.email")}
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
              label={translate("authentication.signUp.password")}
              placeholder={translate("authentication.signUp.passwordHint")}
              autoComplete="new-password"
              value={field.value}
              onChangeText={field.onChange}
              onBlur={field.onBlur}
              errorMessage={fieldState.error?.message}
            />
          )}
        />
      </View>
      <View className="gap-4">
        <SectionTitle title={translate("authentication.signUp.businessSection")} />
        <Controller
          control={form.control}
          name="businessName"
          render={({ field, fieldState }) => (
            <TextField
              label={translate("authentication.signUp.businessName")}
              autoCapitalize="words"
              value={field.value}
              onChangeText={field.onChange}
              onBlur={field.onBlur}
              errorMessage={fieldState.error?.message}
            />
          )}
        />
        <CountryPicker
          selection={{ countryCode, timeZoneId }}
          onSelectionChange={(selection) => {
            form.setValue("countryCode", selection.countryCode);
            form.setValue("timeZoneId", selection.timeZoneId, {
              shouldValidate: form.formState.isSubmitted,
            });
          }}
        />
      </View>
      <Button
        label={translate("authentication.signUp.submit")}
        onPress={submit}
        isLoading={form.formState.isSubmitting}
      />
      <Pressable accessibilityRole="link" onPress={() => router.replace(routes.signIn)}>
        <Text className="text-center text-base font-medium text-brand">
          {translate("authentication.signUp.goToSignIn")}
        </Text>
      </Pressable>
    </AuthenticationScreenLayout>
  );
}
