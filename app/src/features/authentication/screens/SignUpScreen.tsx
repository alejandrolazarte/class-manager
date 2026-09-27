import { zodResolver } from "@hookform/resolvers/zod";
import { useRouter } from "expo-router";
import { useState } from "react";
import { Controller, useForm, useWatch } from "react-hook-form";
import { Pressable, View } from "react-native";
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
import { AppText } from "@/ui/AppText";
import { authenticationLimits } from "@/features/authentication/authenticationLimits";

const badRequestStatus = 400;

type SignUpFailure =
  { kind: "emailTaken"; email: string } | { kind: "network" } | { kind: "unexpected" };

type SignUpStep = 1 | 2;

const signUpStepCount = 2;
const accountStepFieldNames = ["ownerFullName", "email", "password"] as const;

function StepProgress({ step }: { step: SignUpStep }) {
  return (
    <View className="flex-row gap-1.5">
      {[1, 2].map((stepNumber) => (
        <View
          key={stepNumber}
          className={`h-1.5 flex-1 rounded-full ${stepNumber <= step ? "bg-primary" : "bg-muted"}`}
        />
      ))}
    </View>
  );
}

export function SignUpScreen() {
  const router = useRouter();
  const { signUp } = useSession();
  const [signUpFailure, setSignUpFailure] = useState<SignUpFailure | null>(null);
  const [step, setStep] = useState<SignUpStep>(1);
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
  const [countryCode, timeZoneId, password] = useWatch({
    control: form.control,
    name: ["countryCode", "timeZoneId", "password"],
  });
  const hasLongEnoughPassword = password.length >= authenticationLimits.passwordMinimumLength;

  const continueToBusinessStep = async () => {
    if (await form.trigger(accountStepFieldNames)) {
      setStep(2);
    }
  };

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
      if (accountStepFieldNames.some((fieldName) => form.getFieldState(fieldName).invalid)) {
        setStep(1);
      }
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
    <AuthenticationScreenLayout
      eyebrow={translate("authentication.signUp.step", { step, total: signUpStepCount })}
      title={translate(
        step === 1
          ? "authentication.signUp.accountSection"
          : "authentication.signUp.businessSection",
      )}
      onBack={() => (step === 2 ? setStep(1) : router.replace(routes.welcome))}
      progress={<StepProgress step={step} />}
    >
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
      {step === 1 ? (
        <View className="gap-4">
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
                hint={translate("authentication.signUp.passwordHint")}
                isHintSatisfied={hasLongEnoughPassword}
                autoComplete="new-password"
                value={field.value}
                onChangeText={field.onChange}
                onBlur={field.onBlur}
                errorMessage={fieldState.error?.message}
              />
            )}
          />
          <Button label={translate("common.continue")} onPress={continueToBusinessStep} />
        </View>
      ) : (
        <View className="gap-4">
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
          <Button
            label={translate("authentication.signUp.submit")}
            onPress={submit}
            isLoading={form.formState.isSubmitting}
          />
        </View>
      )}
      <Pressable
        accessibilityRole="link"
        className="py-1.5"
        onPress={() => router.replace(routes.signIn)}
      >
        <AppText tone="primary" variant="link" className="text-center">
          {translate("authentication.signUp.goToSignIn")}
        </AppText>
      </Pressable>
    </AuthenticationScreenLayout>
  );
}
