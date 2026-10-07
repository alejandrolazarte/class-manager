import { zodResolver } from "@hookform/resolvers/zod";
import { useRouter } from "expo-router";
import { useState } from "react";
import { Controller, useForm } from "react-hook-form";
import { isApiError } from "@/api/httpClient";
import { clientErrorCodes } from "@/features/clients/clientErrorCodes";
import {
  EditClientFormValues,
  editClientFieldNames,
  editClientRequiredFieldNames,
  editClientSchema,
  toEditClientFormValues,
  toUpdateClientRequest,
} from "@/features/clients/editClientSchema";
import { formatPhoneNumberAsTyped } from "@/features/clients/phoneNumberFormatting";
import { ClientDetails } from "@/features/clients/types";
import { useClient } from "@/features/clients/useClient";
import { useUpdateClient } from "@/features/clients/useUpdateClient";
import { inviteStudentApp } from "@/features/studentApp/studentAppApi";
import { SubmissionFailure, toSubmissionFailure } from "@/features/settings/submissionFailure";
import { applyServerFieldErrors } from "@/forms/applyServerFieldErrors";
import { useRequiredFieldsFilled } from "@/forms/requiredFields";
import { translate } from "@/i18n/translate";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";
import { LoadingScreen } from "@/ui/LoadingScreen";
import { RequiredFieldsLegend } from "@/ui/RequiredFieldsLegend";
import { ScreenFooter, ScrollScreen } from "@/ui/Screen";
import { ScreenHeader } from "@/ui/ScreenHeader";
import { TextField } from "@/ui/TextField";
import { useToast } from "@/ui/ToastProvider";

const badRequestStatus = 400;

interface EditClientScreenProps {
  clientId: string;
}

interface ClientEditorProps {
  client: ClientDetails;
}

function ClientEditor({ client }: ClientEditorProps) {
  const router = useRouter();
  const { showToast } = useToast();
  const updateClientMutation = useUpdateClient(client.id);
  const [submissionFailure, setSubmissionFailure] = useState<SubmissionFailure | null>(null);
  const form = useForm<EditClientFormValues>({
    resolver: zodResolver(editClientSchema),
    defaultValues: toEditClientFormValues(client, client.appAccess.signInEmail),
    mode: "onTouched",
  });
  const areRequiredFieldsFilled = useRequiredFieldsFilled(
    form.control,
    editClientRequiredFieldNames,
  );
  const isInvited = client.appAccess.status === "Invited";
  const usesTheApp = client.appAccess.status === "Active";
  const emailHints = {
    NotInvited: translate("clients.edit.emailHint"),
    Invited: translate("clients.edit.emailHintInvited"),
    Active: translate("clients.edit.emailHintActive"),
    AwaitingGuardianConsent: translate("clients.edit.emailHint"),
  };

  const resendInvitationIfEmailChanged = async (email: string | null) => {
    if (!isInvited || email === null || email === client.appAccess.invitedEmail) {
      return;
    }
    try {
      await inviteStudentApp(client.id, { email });
    } catch {
      showToast(translate("clients.register.invitationFailed"));
    }
  };

  const handleSaveError = (saveError: unknown) => {
    if (isApiError(saveError) && saveError.hasCode(clientErrorCodes.emailUsedToSignIn)) {
      form.setError("email", {
        type: "server",
        message: translate("clients.edit.emailHintActive"),
      });
      return;
    }
    if (isApiError(saveError) && saveError.hasCode(clientErrorCodes.phoneNumberTaken)) {
      form.setError("phoneNumber", {
        type: "server",
        message: translate("clients.edit.phoneTaken"),
      });
      return;
    }
    if (
      isApiError(saveError) &&
      saveError.status === badRequestStatus &&
      applyServerFieldErrors(form, saveError.problem, editClientFieldNames)
    ) {
      return;
    }
    setSubmissionFailure(toSubmissionFailure(saveError));
  };

  const save = form.handleSubmit(async (formValues) => {
    setSubmissionFailure(null);
    const request = toUpdateClientRequest(formValues);
    try {
      await updateClientMutation.mutateAsync(request);
    } catch (saveError) {
      handleSaveError(saveError);
      return;
    }
    await resendInvitationIfEmailChanged(request.email);
    showToast(translate("clients.edit.saved"));
    router.back();
  });

  return (
    <ScrollScreen
      header={<ScreenHeader navigation="close" title={translate("clients.edit.title")} />}
      footer={
        <ScreenFooter>
          <Button
            label={translate("clients.edit.submit")}
            onPress={save}
            disabled={!areRequiredFieldsFilled}
            isLoading={form.formState.isSubmitting}
          />
        </ScreenFooter>
      }
    >
      {submissionFailure === "network" ? (
        <Banner message={translate("common.networkError")}>
          <Button variant="secondary" label={translate("common.retry")} onPress={save} />
        </Banner>
      ) : null}
      {submissionFailure === "unexpected" ? (
        <Banner message={translate("common.unexpectedError")} />
      ) : null}
      <RequiredFieldsLegend />
      <Controller
        control={form.control}
        name="fullName"
        render={({ field, fieldState }) => (
          <TextField
            label={translate("clients.register.fullName")}
            isRequired
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
        name="phoneNumber"
        render={({ field, fieldState }) => (
          <TextField
            label={translate("clients.register.phoneNumber")}
            isRequired
            keyboardType="phone-pad"
            autoComplete="tel"
            value={field.value}
            onChangeText={(typedText) => field.onChange(formatPhoneNumberAsTyped(typedText))}
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
            label={translate("clients.detail.email")}
            keyboardType="email-address"
            autoCapitalize="none"
            autoComplete="email"
            hint={emailHints[client.appAccess.status]}
            editable={!usesTheApp}
            value={field.value}
            onChangeText={field.onChange}
            onBlur={field.onBlur}
            errorMessage={fieldState.error?.message}
          />
        )}
      />
      <Controller
        control={form.control}
        name="notes"
        render={({ field, fieldState }) => (
          <TextField
            label={translate("clients.register.notes")}
            placeholder={translate("clients.register.notesPlaceholder")}
            multiline
            value={field.value}
            onChangeText={field.onChange}
            onBlur={field.onBlur}
            errorMessage={fieldState.error?.message}
          />
        )}
      />
    </ScrollScreen>
  );
}

export function EditClientScreen({ clientId }: EditClientScreenProps) {
  const { data: client } = useClient(clientId);
  if (client === undefined) {
    return <LoadingScreen />;
  }
  return <ClientEditor client={client} />;
}
