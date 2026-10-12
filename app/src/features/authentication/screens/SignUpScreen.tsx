import { zodResolver } from "@hookform/resolvers/zod";
import { useRouter } from "expo-router";
import { useEffect, useState } from "react";
import { Controller, useForm, useWatch } from "react-hook-form";
import { Pressable, View } from "react-native";
import { isApiError, isNetworkError } from "@/api/httpClient";
import { requestSignUp } from "@/features/authentication/authenticationApi";
import { AuthenticationScreenLayout } from "@/features/authentication/components/AuthenticationScreenLayout";
import {
  SignUpFormValues,
  signUpFieldNames,
  signUpSchema,
  toSignUpRequest,
} from "@/features/authentication/signUpSchema";
import { CountryPicker } from "@/features/countries/CountryPicker";
import {
  countrySelectionForTimeZone,
  findCountry,
  getDeviceTimeZone,
} from "@/features/countries/countrySelection";
import { useCountries } from "@/features/countries/useCountries";
import { applyServerFieldErrors } from "@/forms/applyServerFieldErrors";
import { BirthDateField } from "@/forms/BirthDateField";
import { isValidBirthDate } from "@/features/students/birthDateFormatting";
import { translate } from "@/i18n/translate";
import { routes } from "@/navigation/routes";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";
import { IconCircle } from "@/ui/IconCircle";
import { Entrance } from "@/ui/motion/Entrance";
import { TextField } from "@/ui/TextField";
import { AppText } from "@/ui/AppText";
import { RequiredFieldsLegend } from "@/ui/RequiredFieldsLegend";
import { StepProgress } from "@/ui/StepProgress";
import { emailAddressPattern } from "@/forms/emailAddress";
import { useRequiredFieldsFilled } from "@/forms/requiredFields";

const badRequestStatus = 400;

type SignUpFailure = "network" | "unexpected";

type SignUpStep = 1 | 2;

const signUpStepCount = 2;
const accountStepFieldNames = ["ownerFullName", "ownerBirthDate", "email"] as const;
const businessStepFieldNames = ["businessName"] as const;
const startedStepFill = 0.15;
const completedStepFill = 1;

function stepFill(completedFields: number, fieldCount: number): number {
  return startedStepFill + ((completedStepFill - startedStepFill) * completedFields) / fieldCount;
}

export function SignUpScreen() {
  const router = useRouter();
  const [signUpFailure, setSignUpFailure] = useState<SignUpFailure | null>(null);
  const [sentToEmail, setSentToEmail] = useState<string | null>(null);
  const [step, setStep] = useState<SignUpStep>(1);
  const { data: countries = [] } = useCountries();
  const form = useForm<SignUpFormValues>({
    resolver: zodResolver(signUpSchema),
    defaultValues: {
      ownerFullName: "",
      ownerBirthDate: "",
      email: "",
      businessName: "",
      countryCode: "",
      timeZoneId: "",
    },
    mode: "onTouched",
  });
  const [countryCode, timeZoneId, ownerFullName, ownerBirthDate, email, businessName] = useWatch({
    control: form.control,
    name: ["countryCode", "timeZoneId", "ownerFullName", "ownerBirthDate", "email", "businessName"],
  });
  const countryCurrencyCode = findCountry(countries, countryCode)?.currencyCode ?? "";

  useEffect(() => {
    const deviceCountrySelection = countrySelectionForTimeZone(countries, getDeviceTimeZone());
    if (form.getValues("countryCode") === "" && deviceCountrySelection) {
      form.setValue("countryCode", deviceCountrySelection.countryCode);
      form.setValue("timeZoneId", deviceCountrySelection.timeZoneId);
    }
  }, [countries, form]);

  const isAccountStepFilled = useRequiredFieldsFilled(form.control, accountStepFieldNames);
  const isBusinessStepFilled = useRequiredFieldsFilled(form.control, businessStepFieldNames);
  const completedAccountFields = [
    ownerFullName.trim().length > 0,
    isValidBirthDate(ownerBirthDate),
    emailAddressPattern.test(email.trim()),
  ].filter(Boolean).length;
  const segmentFills =
    step === 1
      ? [stepFill(completedAccountFields, accountStepFieldNames.length), 0]
      : [completedStepFill, stepFill(businessName.trim().length > 0 ? 1 : 0, 1)];

  const continueToBusinessStep = async () => {
    if (await form.trigger(accountStepFieldNames)) {
      setStep(2);
    }
  };

  const handleSignUpError = (signUpError: unknown) => {
    if (isNetworkError(signUpError)) {
      setSignUpFailure("network");
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
    setSignUpFailure("unexpected");
  };

  const submit = form.handleSubmit(async (formValues) => {
    setSignUpFailure(null);
    const signUpRequest = toSignUpRequest(formValues, countryCurrencyCode);
    try {
      await requestSignUp(signUpRequest);
      setSentToEmail(signUpRequest.email);
    } catch (signUpError) {
      handleSignUpError(signUpError);
    }
  });

  const changeEmail = () => {
    setSentToEmail(null);
    setStep(1);
  };

  if (sentToEmail) {
    return (
      <AuthenticationScreenLayout
        title={translate("authentication.signUp.checkEmailTitle")}
        onBack={changeEmail}
      >
        <Entrance kind="pop" className="self-start">
          <IconCircle icon="email" tone="success" size="large" />
        </Entrance>
        <AppText tone="muted" accessibilityRole="alert">
          {translate("authentication.signUp.checkEmail", { email: sentToEmail })}
        </AppText>
        <Button
          variant="secondary"
          label={translate("authentication.signUp.changeEmail")}
          onPress={changeEmail}
        />
      </AuthenticationScreenLayout>
    );
  }

  return (
    <AuthenticationScreenLayout
      eyebrow={translate("authentication.signUp.step", { step, total: signUpStepCount })}
      title={translate(
        step === 1
          ? "authentication.signUp.accountSection"
          : "authentication.signUp.businessSection",
      )}
      onBack={() => (step === 2 ? setStep(1) : router.replace(routes.welcome))}
      progress={<StepProgress segmentFills={segmentFills} />}
      footer={
        <Pressable
          accessibilityRole="link"
          className="py-1.5"
          onPress={() => router.replace(routes.signIn)}
        >
          <AppText tone="primary" variant="link" className="text-center">
            {translate("authentication.signUp.goToSignIn")}
          </AppText>
        </Pressable>
      }
    >
      {signUpFailure === "network" ? (
        <Banner message={translate("common.networkError")}>
          <Button variant="secondary" label={translate("common.retry")} onPress={submit} />
        </Banner>
      ) : null}
      {signUpFailure === "unexpected" ? (
        <Banner message={translate("common.unexpectedError")} />
      ) : null}
      <RequiredFieldsLegend />
      {step === 1 ? (
        <View className="gap-4">
          <Controller
            control={form.control}
            name="ownerFullName"
            render={({ field, fieldState }) => (
              <TextField
                label={translate("authentication.signUp.ownerFullName")}
                isRequired
                placeholder={translate("authentication.signUp.ownerFullNamePlaceholder")}
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
            name="ownerBirthDate"
            render={({ field, fieldState }) => (
              <BirthDateField
                hint={translate("authentication.signUp.birthDateHint")}
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
                isRequired
                placeholder={translate("authentication.signUp.emailPlaceholder")}
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
          <Button
            label={translate("common.continue")}
            trailingIcon="forward"
            onPress={continueToBusinessStep}
            disabled={!isAccountStepFilled}
          />
        </View>
      ) : (
        <View className="gap-4">
          <Controller
            control={form.control}
            name="businessName"
            render={({ field, fieldState }) => (
              <TextField
                label={translate("authentication.signUp.businessName")}
                isRequired
                placeholder={translate("authentication.signUp.businessNamePlaceholder")}
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
            disabled={!isBusinessStepFilled || countryCurrencyCode === ""}
            isLoading={form.formState.isSubmitting}
          />
        </View>
      )}
    </AuthenticationScreenLayout>
  );
}
