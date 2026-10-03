import { zodResolver } from "@hookform/resolvers/zod";
import { useRouter } from "expo-router";
import { useState } from "react";
import { useForm } from "react-hook-form";
import { View } from "react-native";
import { getFieldErrors, getStringExtension } from "@/api/problemDetails";
import { isApiError, isNetworkError } from "@/api/httpClient";
import {
  clientErrorCodes,
  phoneNumberTakenClientIdExtension,
} from "@/features/clients/clientErrorCodes";
import { ClientForm } from "@/features/clients/components/ClientForm";
import {
  emptyRegisterClientFormValues,
  RegisterClientFormValues,
  registerClientSchema,
  toAppInvitationEmail,
  toRegisterClientFieldName,
  toRegisterClientRequest,
} from "@/features/clients/registerClientSchema";
import { ClientDetails } from "@/features/clients/types";
import { useClient } from "@/features/clients/useClient";
import { useRegisterClient } from "@/features/clients/useRegisterClient";
import { inviteFamily } from "@/features/family/familyApi";
import { translate } from "@/i18n/translate";
import { routes } from "@/navigation/routes";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";
import { useToast } from "@/ui/ToastProvider";
import { ScrollScreen } from "@/ui/Screen";
import { ScreenHeader } from "@/ui/ScreenHeader";

const badRequestStatus = 400;
const singleStudent = 1;

type SubmissionFailure =
  | { kind: "phoneNumberTaken"; existingClientId: string }
  | { kind: "network" }
  | { kind: "unexpected" };

export function RegisterClientScreen() {
  const router = useRouter();
  const { showToast } = useToast();
  const registerClientMutation = useRegisterClient();
  const [submissionFailure, setSubmissionFailure] = useState<SubmissionFailure | null>(null);
  const form = useForm<RegisterClientFormValues>({
    resolver: zodResolver(registerClientSchema),
    defaultValues: emptyRegisterClientFormValues,
    mode: "onTouched",
  });
  const existingClientId =
    submissionFailure?.kind === "phoneNumberTaken" ? submissionFailure.existingClientId : undefined;
  const { data: existingClient } = useClient(existingClientId);

  const registrationSuccessMessage = (
    registeredClient: ClientDetails,
    isAppInvitationSent: boolean,
  ): string => {
    const hasSeveralStudents = registeredClient.students.length > singleStudent;
    if (isAppInvitationSent) {
      return hasSeveralStudents
        ? translate("clients.register.successPluralWithInvitation")
        : translate("clients.register.successWithInvitation");
    }
    return hasSeveralStudents
      ? translate("clients.register.successPlural")
      : translate("clients.register.success");
  };

  const sendAppInvitation = async (
    registeredClient: ClientDetails,
    appInvitationEmail: string,
  ): Promise<string> => {
    try {
      await inviteFamily(registeredClient.id, { email: appInvitationEmail });
      return registrationSuccessMessage(registeredClient, true);
    } catch {
      return translate("clients.register.invitationFailed");
    }
  };

  const navigateAfterRegistration = async (
    registeredClient: ClientDetails,
    appInvitationEmail: string | null,
  ) => {
    showToast(
      appInvitationEmail === null
        ? registrationSuccessMessage(registeredClient, false)
        : await sendAppInvitation(registeredClient, appInvitationEmail),
    );
    router.replace(routes.clientDetail("students", registeredClient.id));
  };

  const handleRegistrationError = (registrationError: unknown) => {
    if (isNetworkError(registrationError)) {
      setSubmissionFailure({ kind: "network" });
      return;
    }
    if (
      isApiError(registrationError) &&
      registrationError.hasCode(clientErrorCodes.phoneNumberTaken)
    ) {
      const conflictingClientId = getStringExtension(
        registrationError.problem,
        phoneNumberTakenClientIdExtension,
      );
      setSubmissionFailure(
        conflictingClientId
          ? { kind: "phoneNumberTaken", existingClientId: conflictingClientId }
          : { kind: "unexpected" },
      );
      return;
    }
    if (isApiError(registrationError) && registrationError.status === badRequestStatus) {
      const clientAttends = form.getValues("clientAttends");
      const fieldErrors = Object.entries(getFieldErrors(registrationError.problem)).flatMap(
        ([serverFieldName, message]) => {
          const formFieldName = toRegisterClientFieldName(serverFieldName, clientAttends);
          return formFieldName === null ? [] : [{ formFieldName, message }];
        },
      );
      fieldErrors.forEach(({ formFieldName, message }) => {
        form.setError(formFieldName, { type: "server", message });
      });
      if (fieldErrors.length > 0) {
        return;
      }
    }
    setSubmissionFailure({ kind: "unexpected" });
  };

  const submit = form.handleSubmit(async (formValues) => {
    setSubmissionFailure(null);
    let registeredClient: ClientDetails;
    try {
      registeredClient = await registerClientMutation.mutateAsync(
        toRegisterClientRequest(formValues),
      );
    } catch (registrationError) {
      handleRegistrationError(registrationError);
      return;
    }
    await navigateAfterRegistration(registeredClient, toAppInvitationEmail(formValues));
  });

  return (
    <ScrollScreen
      header={<ScreenHeader navigation="close" title={translate("clients.register.title")} />}
    >
      {submissionFailure?.kind === "phoneNumberTaken" ? (
        <Banner
          tone="warning"
          message={
            existingClient
              ? translate("clients.register.phoneTaken", { name: existingClient.fullName })
              : translate("clients.register.phoneTakenUnknownName")
          }
        >
          <Button
            variant="secondary"
            label={translate("clients.register.openClient")}
            onPress={() =>
              router.push(routes.clientDetail("students", submissionFailure.existingClientId))
            }
          />
        </Banner>
      ) : null}
      {submissionFailure?.kind === "network" ? (
        <Banner message={translate("common.networkError")}>
          <Button variant="secondary" label={translate("common.retry")} onPress={submit} />
        </Banner>
      ) : null}
      {submissionFailure?.kind === "unexpected" ? (
        <Banner message={translate("common.unexpectedError")} />
      ) : null}
      <View>
        <ClientForm form={form} onSubmit={submit} isSubmitting={form.formState.isSubmitting} />
      </View>
    </ScrollScreen>
  );
}
