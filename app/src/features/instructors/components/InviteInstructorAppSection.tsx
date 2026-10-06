import { useState } from "react";
import { View } from "react-native";
import { isApiError } from "@/api/httpClient";
import { instructorAppAccess } from "@/features/instructors/instructorAppAccess";
import { Instructor, InstructorAppAccessStatus } from "@/features/instructors/types";
import { memberErrorCodes } from "@/features/members/memberErrorCodes";
import { useTeam } from "@/features/members/useTeam";
import { useInviteMember, useResendInvitation } from "@/features/members/useTeamMutations";
import { emailAddressPattern } from "@/forms/emailAddress";
import { translate } from "@/i18n/translate";
import { AppText, TextTone } from "@/ui/AppText";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";
import { Card } from "@/ui/Card";
import { Icon } from "@/ui/Icon";
import { Spinner } from "@/ui/Spinner";
import { TextField } from "@/ui/TextField";
import { useToast } from "@/ui/ToastProvider";

const coachRole = "Coach";

const statusTones: Record<InstructorAppAccessStatus, TextTone> = {
  NotInvited: "muted",
  Invited: "warning",
  Active: "success",
};

interface InviteInstructorAppSectionProps {
  instructor: Instructor;
}

export function InviteInstructorAppSection({ instructor }: InviteInstructorAppSectionProps) {
  const { showToast } = useToast();
  const teamQuery = useTeam();
  const inviteMutation = useInviteMember();
  const resendMutation = useResendInvitation();
  const emailOnFile = (instructor.email ?? "").trim();
  const [isAskingEmail, setIsAskingEmail] = useState(false);
  const [email, setEmail] = useState(emailOnFile);
  const [emailError, setEmailError] = useState<string | undefined>();
  const [failureMessage, setFailureMessage] = useState<string | undefined>();

  if (teamQuery.isPending) {
    return <Spinner />;
  }
  if (teamQuery.isError) {
    return (
      <Banner message={translate("common.unexpectedError")}>
        <Button
          variant="secondary"
          size="medium"
          label={translate("common.retry")}
          onPress={() => teamQuery.refetch()}
        />
      </Banner>
    );
  }

  const access = instructorAppAccess(instructor.id, teamQuery.data);
  const isSending = inviteMutation.isPending || resendMutation.isPending;
  const accessEmail = access.member?.email ?? access.invitation?.email ?? "";

  const sendTo = async (recipientEmail: string) => {
    setFailureMessage(undefined);
    if (!emailAddressPattern.test(recipientEmail)) {
      setIsAskingEmail(true);
      setEmailError(translate("team.invite.emailInvalid"));
      return;
    }
    setEmailError(undefined);
    try {
      await inviteMutation.mutateAsync({
        email: recipientEmail,
        role: coachRole,
        customRoleId: null,
        instructorId: instructor.id,
      });
      showToast(translate("team.invite.sent"));
      setIsAskingEmail(false);
    } catch (inviteError) {
      if (isApiError(inviteError) && inviteError.hasCode(memberErrorCodes.alreadyMember)) {
        setIsAskingEmail(true);
        setEmailError(translate("team.invite.alreadyMember"));
        return;
      }
      if (isApiError(inviteError) && inviteError.hasCode(memberErrorCodes.instructorTaken)) {
        setFailureMessage(translate("team.invite.coachTaken"));
        return;
      }
      setFailureMessage(translate("common.unexpectedError"));
    }
  };

  const resend = async (invitationId: string) => {
    setFailureMessage(undefined);
    try {
      await resendMutation.mutateAsync(invitationId);
      showToast(translate("team.resent"));
    } catch {
      setFailureMessage(translate("common.unexpectedError"));
    }
  };

  const invite = () => {
    if (access.invitation !== undefined) {
      void resend(access.invitation.id);
      return;
    }
    if (emailOnFile.length === 0) {
      setIsAskingEmail(true);
      return;
    }
    void sendTo(emailOnFile);
  };

  return (
    <Card className="gap-3 p-3.5">
      <View className="flex-row items-center gap-3">
        <View className="h-10 w-10 items-center justify-center rounded-xl bg-primary-soft">
          <Icon name="instructors" size="medium" tone="primary" />
        </View>
        <View className="min-w-0 flex-1 gap-0.5">
          <AppText variant="bodyStrong">{translate("instructors.app.title")}</AppText>
          <AppText variant="caption" tone={statusTones[access.status]}>
            {translate(`instructors.app.${access.status}`, { email: accessEmail })}
          </AppText>
        </View>
        {access.status === "Active" || isAskingEmail ? null : (
          <Button
            size="medium"
            label={translate(
              access.status === "Invited" ? "instructors.app.resend" : "instructors.app.invite",
            )}
            accessibilityLabel={translate(
              access.status === "Invited"
                ? "instructors.app.resendAccessibility"
                : "instructors.app.inviteAccessibility",
            )}
            onPress={invite}
            isLoading={isSending}
          />
        )}
      </View>
      {failureMessage ? <Banner message={failureMessage} /> : null}
      {isAskingEmail ? (
        <View className="gap-2.5 rounded-2xl bg-muted p-3">
          <AppText variant="caption" tone="subtle">
            {translate("instructors.invite.hint")}
          </AppText>
          <TextField
            label={translate("instructors.invite.email")}
            isRequired
            keyboardType="email-address"
            autoCapitalize="none"
            autoComplete="email"
            fieldSurface="background"
            value={email}
            onChangeText={setEmail}
            errorMessage={emailError}
          />
          <Button
            size="medium"
            label={translate("team.invite.submit")}
            onPress={() => sendTo(email.trim())}
            disabled={email.trim().length === 0}
            isLoading={isSending}
          />
        </View>
      ) : null}
    </Card>
  );
}
