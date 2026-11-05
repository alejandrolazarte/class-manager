import { zodResolver } from "@hookform/resolvers/zod";
import { useRouter } from "expo-router";
import { useState } from "react";
import { useForm } from "react-hook-form";
import { View } from "react-native";
import { getFieldErrors, getStringExtension, toCamelCase } from "@/api/problemDetails";
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
import { ClientDetails, RegisterClientRequest } from "@/features/clients/types";
import { useClient } from "@/features/clients/useClient";
import { useRegisterClient } from "@/features/clients/useRegisterClient";
import { inviteStudentApp } from "@/features/studentApp/studentAppApi";
import { SamePersonDialog } from "@/features/users/components/SamePersonDialog";
import { SamePerson, samePersonOf } from "@/features/users/samePerson";
import { translate } from "@/i18n/translate";
import { routes } from "@/navigation/routes";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";
import { useToast } from "@/ui/ToastProvider";
import { ScrollScreen } from "@/ui/Screen";
import { ScreenHeader } from "@/ui/ScreenHeader";

const badRequestStatus = 400;
const singleStudent = 1;
const contactEmailField = "Email";

type ConfirmedSamePeople = Record<string, string>;

function withSamePeople(
  request: RegisterClientRequest,
  confirmedSamePeople: ConfirmedSamePeople,
): RegisterClientRequest {
  const contactSamePersonUserId = confirmedSamePeople[contactEmailField];
  return {
    ...request,
    ...(contactSamePersonUserId ? { samePersonUserId: contactSamePersonUserId } : {}),
    students: request.students.map((student, index) => {
      const samePersonUserId = confirmedSamePeople[`Students[${index}].Email`];
      return samePersonUserId ? { ...student, samePersonUserId } : student;
    }),
  };
}

type SubmissionFailure =
  | { kind: "phoneNumberTaken"; existingClientId: string }
  | { kind: "network" }
  | { kind: "unexpected" };

export function RegisterClientScreen() {
  const router = useRouter();
  const { showToast } = useToast();
  const registerClientMutation = useRegisterClient();
  const [submissionFailure, setSubmissionFailure] = useState<SubmissionFailure | null>(null);
  const [samePerson, setSamePerson] = useState<SamePerson | null>(null);
  const [confirmedSamePeople, setConfirmedSamePeople] = useState<ConfirmedSamePeople>({});
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
      await inviteStudentApp(registeredClient.id, {
        email: appInvitationEmail,
        confirmsContactIsAdult: true,
      });
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

  const register = async (
    formValues: RegisterClientFormValues,
    samePeople: ConfirmedSamePeople,
  ) => {
    setSubmissionFailure(null);
    const request = toRegisterClientRequest(formValues);
    let registeredClient: ClientDetails;
    try {
      registeredClient = await registerClientMutation.mutateAsync(
        withSamePeople(request, samePeople),
      );
    } catch (registrationError) {
      const conflictingPerson = samePersonOf(registrationError, "");
      setSamePerson(
        conflictingPerson && {
          ...conflictingPerson,
          email: emailOfField(request, conflictingPerson.field),
        },
      );
      if (conflictingPerson === null) {
        handleRegistrationError(registrationError);
      }
      return;
    }
    setSamePerson(null);
    await navigateAfterRegistration(registeredClient, toAppInvitationEmail(formValues));
  };

  const submit = form.handleSubmit((formValues) => register(formValues, confirmedSamePeople));

  const joinSamePerson = (person: SamePerson) => {
    const samePeople = { ...confirmedSamePeople, [person.field]: person.userId };
    setConfirmedSamePeople(samePeople);
    void register(form.getValues(), samePeople);
  };

  const keepApart = (person: SamePerson) => {
    setSamePerson(null);
    const formFieldName = toRegisterClientFieldName(
      toCamelCase(person.field),
      form.getValues("clientAttends"),
    );
    if (formFieldName !== null) {
      form.setError(formFieldName, {
        type: "server",
        message: translate("apiErrors.user.email_taken"),
      });
    }
  };

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
      {samePerson ? (
        <SamePersonDialog
          person={samePerson}
          messageKey="people.samePerson.inTeam"
          onCancel={() => keepApart(samePerson)}
          onConfirm={() => joinSamePerson(samePerson)}
          isConfirming={registerClientMutation.isPending}
        />
      ) : null}
      <View>
        <ClientForm form={form} onSubmit={submit} isSubmitting={form.formState.isSubmitting} />
      </View>
    </ScrollScreen>
  );
}

function emailOfField(request: RegisterClientRequest, field: string): string {
  if (field === contactEmailField) {
    return request.email ?? "";
  }
  const studentIndex = Number(/^Students\[(\d+)\]/.exec(field)?.[1] ?? Number.NaN);
  return request.students[studentIndex]?.email ?? "";
}
