import { useCallback } from "react";
import { checkInvitation, declineInvitation } from "@/features/authentication/authenticationApi";
import {
  AcceptInvitationAnswer,
  InvitationScreen,
} from "@/features/authentication/components/InvitationScreen";
import { CheckedInvitation } from "@/features/authentication/types";
import { useSession } from "@/features/authentication/useSession";
import { memberErrorCodes } from "@/features/members/memberErrorCodes";
import { translate } from "@/i18n/translate";
import { routes } from "@/navigation/routes";

const conflictMessages = {
  [memberErrorCodes.instructorTaken]: translate("invitation.coachTaken"),
  [memberErrorCodes.alreadyMember]: translate("team.invite.alreadyMember"),
};

function textsFor(invitation: CheckedInvitation) {
  return {
    newAccountSubtitle: translate("invitation.subtitle"),
    existingAccount: translate("invitation.existingAccount", {
      business: invitation.businessName,
      email: invitation.email,
    }),
    fullName: translate("invitation.fullName"),
    password: translate("invitation.password"),
    passwordHint: translate("invitation.passwordHint"),
    createAccount: translate("invitation.submit"),
  };
}

export function AcceptInvitationScreen() {
  const { acceptInvitation } = useSession();
  const accept = useCallback(
    (answer: AcceptInvitationAnswer) => acceptInvitation(answer),
    [acceptInvitation],
  );

  return (
    <InvitationScreen
      title={translate("invitation.title")}
      invalidMessage={translate("invitation.invalid")}
      invalidInvitationCode={memberErrorCodes.invalidInvitation}
      conflictMessages={conflictMessages}
      successRoute={routes.today}
      textsFor={textsFor}
      check={checkInvitation}
      accept={accept}
      decline={declineInvitation}
    />
  );
}
