import { useQueryClient } from "@tanstack/react-query";
import { useState } from "react";
import { View } from "react-native";
import { clientQueryKeys } from "@/features/clients/clientQueryKeys";
import { isApiError } from "@/api/httpClient";
import { ClientDetails, StudentAppAccessStatus } from "@/features/clients/types";
import { inviteStudentApp } from "@/features/studentApp/studentAppApi";
import {
  formatBirthDateAsTyped,
  isValidBirthDate,
  parseBirthDate,
} from "@/features/students/birthDateFormatting";
import { studentErrorCodes } from "@/features/students/studentErrorCodes";
import { isEmailOfAnotherPerson } from "@/features/students/studentSchema";
import { Student } from "@/features/students/types";
import { emailAddressPattern } from "@/forms/emailAddress";
import { translate } from "@/i18n/translate";
import { AppText, TextTone } from "@/ui/AppText";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";
import { Card } from "@/ui/Card";
import { Icon } from "@/ui/Icon";
import { TextField } from "@/ui/TextField";
import { useToast } from "@/ui/ToastProvider";

interface InviteStudentAppSectionProps {
  client: ClientDetails;
  student?: Student;
}

const statusTones: Record<StudentAppAccessStatus, TextTone> = {
  NotInvited: "muted",
  Invited: "warning",
  Active: "success",
  AwaitingGuardianConsent: "warning",
};

function otherPeopleEmails(client: ClientDetails, student: Student | undefined): (string | null)[] {
  const otherStudents = client.students.filter((familyStudent) => familyStudent.id !== student?.id);
  return [
    ...(student ? [client.email, client.appAccess.invitedEmail] : []),
    ...otherStudents.flatMap((otherStudent) => [
      otherStudent.email,
      otherStudent.appAccess.invitedEmail,
    ]),
  ];
}

export function InviteStudentAppSection({ client, student }: InviteStudentAppSectionProps) {
  const { showToast } = useToast();
  const queryClient = useQueryClient();
  const { status, invitedEmail } = student ? student.appAccess : client.appAccess;
  const isPending = status === "Invited" || status === "AwaitingGuardianConsent";
  const invitedRecipientEmail = status === "AwaitingGuardianConsent" ? null : invitedEmail;
  const emailOnFile = (
    invitedRecipientEmail ??
    (student ? student.email : client.email) ??
    ""
  ).trim();
  const needsBirthDate = student !== undefined && student.birthDate === null;
  const [isAskingEmail, setIsAskingEmail] = useState(false);
  const [email, setEmail] = useState(emailOnFile);
  const [emailError, setEmailError] = useState<string | undefined>();
  const [birthDate, setBirthDate] = useState("");
  const [birthDateError, setBirthDateError] = useState<string | undefined>();
  const [hasFailed, setHasFailed] = useState(false);
  const [isSending, setIsSending] = useState(false);

  const askAgain = (emailMessage: string | undefined, birthDateMessage: string | undefined) => {
    setIsAskingEmail(true);
    setEmailError(emailMessage);
    setBirthDateError(birthDateMessage);
  };

  const sendTo = async (recipientEmail: string) => {
    setHasFailed(false);
    const emailMessage = !emailAddressPattern.test(recipientEmail)
      ? translate("student.invite.emailInvalid")
      : isEmailOfAnotherPerson(recipientEmail, otherPeopleEmails(client, student))
        ? translate("students.validation.emailOfAnotherPerson")
        : undefined;
    const birthDateMessage =
      needsBirthDate && !isValidBirthDate(birthDate)
        ? translate("student.invite.birthDateInvalid")
        : undefined;
    if (emailMessage || birthDateMessage) {
      askAgain(emailMessage, birthDateMessage);
      return;
    }
    setEmailError(undefined);
    setBirthDateError(undefined);
    setIsSending(true);
    try {
      const invitation = await inviteStudentApp(client.id, {
        email: recipientEmail,
        ...(student ? { studentId: student.id } : {}),
        ...(needsBirthDate ? { birthDate: parseBirthDate(birthDate) ?? undefined } : {}),
      });
      showToast(
        invitation.awaitsGuardianConsent
          ? translate("student.invite.guardianAsked", { email: client.email ?? "" })
          : translate("student.invite.sent"),
      );
      setIsAskingEmail(false);
      await queryClient.invalidateQueries({ queryKey: clientQueryKeys.detail(client.id) });
    } catch (inviteError) {
      if (isApiError(inviteError) && inviteError.hasCode(studentErrorCodes.emailOfAnotherPerson)) {
        askAgain(translate("students.validation.emailOfAnotherPerson"), undefined);
        return;
      }
      setHasFailed(true);
    } finally {
      setIsSending(false);
    }
  };

  const invite = () => {
    if (isPending && emailOnFile.length > 0 && !needsBirthDate) {
      void sendTo(emailOnFile);
      return;
    }
    setEmail(emailOnFile);
    setIsAskingEmail(true);
  };

  const cancel = () => {
    setEmailError(undefined);
    setBirthDateError(undefined);
    setIsAskingEmail(false);
  };

  const isFormFilled = email.trim().length > 0 && (!needsBirthDate || birthDate.length > 0);
  const Container = student ? View : Card;

  return (
    <Container className={student ? "gap-3 rounded-2xl bg-muted p-3" : "gap-3 p-3.5"}>
      <View className="flex-row items-center gap-3">
        <View className="h-10 w-10 items-center justify-center rounded-xl bg-primary-soft">
          <Icon name="studentApp" size="medium" tone="primary" />
        </View>
        <View className="min-w-0 flex-1 gap-0.5">
          <AppText variant="bodyStrong">{translate("student.app.title")}</AppText>
          <AppText variant="caption" tone={statusTones[status]}>
            {translate(`student.app.${status}`, { email: invitedEmail ?? "" })}
          </AppText>
        </View>
        {status === "Active" || isAskingEmail ? null : (
          <Button
            size="medium"
            label={translate(isPending ? "student.app.resend" : "student.app.invite")}
            accessibilityLabel={translate(
              isPending ? "student.app.resendAccessibility" : "student.app.inviteAccessibility",
            )}
            onPress={invite}
            isLoading={isSending}
          />
        )}
      </View>
      {hasFailed ? <Banner message={translate("common.unexpectedError")} /> : null}
      {isAskingEmail ? (
        <View className={student ? "gap-2.5" : "gap-2.5 rounded-2xl bg-muted p-3"}>
          <AppText variant="caption" tone="subtle">
            {translate(
              emailOnFile.length > 0
                ? "student.invite.emailOnFileHint"
                : student
                  ? "student.invite.studentHint"
                  : "student.invite.hint",
            )}
          </AppText>
          <TextField
            label={
              student
                ? translate("student.invite.studentEmail", { name: student.fullName })
                : translate("student.invite.email")
            }
            isRequired
            keyboardType="email-address"
            autoCapitalize="none"
            autoComplete="email"
            fieldSurface="background"
            value={email}
            onChangeText={setEmail}
            errorMessage={emailError}
          />
          {needsBirthDate ? (
            <TextField
              label={translate("student.invite.birthDate")}
              hint={translate("student.invite.birthDateHint")}
              isRequired
              placeholder={translate("students.fields.birthDatePlaceholder")}
              keyboardType="number-pad"
              fieldSurface="background"
              value={birthDate}
              onChangeText={(typedText) => setBirthDate(formatBirthDateAsTyped(typedText))}
              errorMessage={birthDateError}
            />
          ) : null}
          <Button
            size="medium"
            label={translate("student.invite.send")}
            onPress={() => sendTo(email.trim())}
            disabled={!isFormFilled}
            isLoading={isSending}
          />
          <Button
            variant="ghost"
            size="medium"
            label={translate("common.cancel")}
            onPress={cancel}
          />
        </View>
      ) : null}
    </Container>
  );
}
