import { useCallback } from "react";
import {
  checkStudentAppInvitation,
  declineStudentAppInvitation,
} from "@/features/authentication/authenticationApi";
import {
  AcceptInvitationAnswer,
  InvitationScreen,
} from "@/features/authentication/components/InvitationScreen";
import { CheckedInvitation } from "@/features/authentication/types";
import { useSession } from "@/features/authentication/useSession";
import { studentAppErrorCodes } from "@/features/studentApp/studentAppErrorCodes";
import { translate } from "@/i18n/translate";
import { routes } from "@/navigation/routes";

const conflictMessages = {
  [studentAppErrorCodes.alreadyLinked]: translate("studentAppInvitation.alreadyLinked"),
};

function textsFor(invitation: CheckedInvitation) {
  return {
    newAccountSubtitle: translate("studentAppInvitation.subtitle"),
    existingAccount: translate("studentAppInvitation.existingAccount", {
      business: invitation.businessName,
      email: invitation.email,
    }),
    fullName: translate("studentAppInvitation.fullName"),
    password: translate("studentAppInvitation.password"),
    passwordHint: translate("studentAppInvitation.passwordHint"),
    createAccount: translate("studentAppInvitation.submit"),
  };
}

export function AcceptStudentAppInvitationScreen() {
  const { acceptStudentAppInvitation } = useSession();
  const accept = useCallback(
    (answer: AcceptInvitationAnswer) => acceptStudentAppInvitation(answer),
    [acceptStudentAppInvitation],
  );

  return (
    <InvitationScreen
      title={translate("studentAppInvitation.title")}
      existingAccountTitle={translate("studentAppInvitation.existingAccountTitle")}
      invalidMessage={translate("studentAppInvitation.invalid")}
      invalidInvitationCode={studentAppErrorCodes.invalidInvitation}
      conflictMessages={conflictMessages}
      successRoute={routes.studentApp}
      textsFor={textsFor}
      check={checkStudentAppInvitation}
      accept={accept}
      decline={declineStudentAppInvitation}
    />
  );
}
