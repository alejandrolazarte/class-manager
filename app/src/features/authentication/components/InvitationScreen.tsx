import { useLocalSearchParams, useRouter } from "expo-router";
import { useEffect, useState } from "react";
import { isApiError, isNetworkError } from "@/api/httpClient";
import { getFieldErrors } from "@/api/problemDetails";
import { authenticationErrorCodes } from "@/features/authentication/authenticationErrorCodes";
import { AuthenticationScreenLayout } from "@/features/authentication/components/AuthenticationScreenLayout";
import {
  InvitationAnswerForm,
  InvitationAnswerTexts,
  InvitationFieldErrors,
} from "@/features/authentication/components/InvitationAnswerForm";
import { CheckedInvitation, CheckInvitationRequest } from "@/features/authentication/types";
import { translate } from "@/i18n/translate";
import { routes } from "@/navigation/routes";
import { Banner } from "@/ui/Banner";
import { BrandMark } from "@/ui/BrandMark";
import { Button } from "@/ui/Button";
import { Spinner } from "@/ui/Spinner";

export interface AcceptInvitationAnswer {
  token: string;
  fullName: string;
  password: string;
  birthDate?: string;
}

interface InvitationScreenProps {
  title: string;
  invalidMessage: string;
  invalidInvitationCode: string;
  conflictMessages: Record<string, string>;
  successRoute: string;
  textsFor: (invitation: CheckedInvitation) => InvitationAnswerTexts;
  check: (request: CheckInvitationRequest) => Promise<CheckedInvitation>;
  accept: (answer: AcceptInvitationAnswer) => Promise<void>;
  decline: (request: CheckInvitationRequest) => Promise<void>;
}

type InvitationFailure =
  | { kind: "invalidLink" }
  | { kind: "message"; message: string }
  | { kind: "network"; retry: () => void };

const badRequestStatus = 400;

export function InvitationScreen({
  title,
  invalidMessage,
  invalidInvitationCode,
  conflictMessages,
  successRoute,
  textsFor,
  check,
  accept,
  decline,
}: InvitationScreenProps) {
  const router = useRouter();
  const { token } = useLocalSearchParams<{ token?: string }>();
  const [invitation, setInvitation] = useState<CheckedInvitation | null>(null);
  const [fieldErrors, setFieldErrors] = useState<InvitationFieldErrors>({});
  const [isAccepting, setIsAccepting] = useState(false);
  const [isDeclining, setIsDeclining] = useState(false);
  const [isDeclined, setIsDeclined] = useState(false);
  const [failure, setFailure] = useState<InvitationFailure | null>(
    token ? null : { kind: "invalidLink" },
  );
  const [isChecking, setIsChecking] = useState(Boolean(token));

  useEffect(() => {
    if (!token) {
      return;
    }
    let isCurrent = true;
    const verifyLink = async () => {
      try {
        const checkedInvitation = await check({ token });
        if (isCurrent) {
          setInvitation(checkedInvitation);
        }
      } catch (checkError) {
        if (isCurrent) {
          setFailure(
            isApiError(checkError) && checkError.hasCode(invalidInvitationCode)
              ? { kind: "invalidLink" }
              : { kind: "message", message: translate("common.unexpectedError") },
          );
        }
      } finally {
        if (isCurrent) {
          setIsChecking(false);
        }
      }
    };
    void verifyLink();
    return () => {
      isCurrent = false;
    };
  }, [token, check, invalidInvitationCode]);

  const failureOf = (answerError: unknown, retry: () => void): InvitationFailure | null => {
    if (isNetworkError(answerError)) {
      return { kind: "network", retry };
    }
    if (isApiError(answerError) && answerError.hasCode(invalidInvitationCode)) {
      return { kind: "invalidLink" };
    }
    if (
      isApiError(answerError) &&
      answerError.hasCode(authenticationErrorCodes.tooYoungForOwnAccount)
    ) {
      setFieldErrors({ birthDate: translate("invitationAnswer.tooYoung") });
      return null;
    }
    const conflictCode = Object.keys(conflictMessages).find(
      (code) => isApiError(answerError) && answerError.hasCode(code),
    );
    if (conflictCode) {
      return { kind: "message", message: conflictMessages[conflictCode] };
    }
    if (isApiError(answerError) && answerError.status === badRequestStatus) {
      const serverFieldErrors = getFieldErrors(answerError.problem);
      if (serverFieldErrors.fullName || serverFieldErrors.password) {
        setFieldErrors({
          fullName: serverFieldErrors.fullName,
          password: serverFieldErrors.password,
        });
        return null;
      }
    }
    return { kind: "message", message: translate("common.unexpectedError") };
  };

  const acceptInvitation = async (fullName: string, password: string, birthDate?: string) => {
    if (!token) {
      return;
    }
    setFailure(null);
    setFieldErrors({});
    setIsAccepting(true);
    try {
      await accept({ token, fullName, password, ...(birthDate ? { birthDate } : {}) });
      router.replace(successRoute);
    } catch (acceptError) {
      setFailure(
        failureOf(acceptError, () => void acceptInvitation(fullName, password, birthDate)),
      );
    } finally {
      setIsAccepting(false);
    }
  };

  const declineInvitation = async () => {
    if (!token) {
      return;
    }
    setFailure(null);
    setIsDeclining(true);
    try {
      await decline({ token });
      setIsDeclined(true);
    } catch (declineError) {
      setFailure(failureOf(declineError, () => void declineInvitation()));
    } finally {
      setIsDeclining(false);
    }
  };

  const goToSignIn = () => router.replace(routes.signIn);

  return (
    <AuthenticationScreenLayout title={title} onBack={goToSignIn} mark={<BrandMark />}>
      {failure?.kind === "invalidLink" ? (
        <Banner message={invalidMessage}>
          <Button
            variant="secondary"
            label={translate("invitation.goToSignIn")}
            onPress={goToSignIn}
          />
        </Banner>
      ) : null}
      {failure?.kind === "message" ? <Banner message={failure.message} /> : null}
      {failure?.kind === "network" ? (
        <Banner message={translate("common.networkError")}>
          <Button variant="secondary" label={translate("common.retry")} onPress={failure.retry} />
        </Banner>
      ) : null}
      {isDeclined ? (
        <Banner tone="info" message={translate("invitationAnswer.declined")}>
          <Button
            variant="secondary"
            label={translate("invitation.goToSignIn")}
            onPress={goToSignIn}
          />
        </Banner>
      ) : null}
      {isChecking ? <Spinner className="my-4" /> : null}
      {invitation && !isDeclined && failure?.kind !== "invalidLink" ? (
        <InvitationAnswerForm
          invitation={invitation}
          texts={textsFor(invitation)}
          fieldErrors={fieldErrors}
          isAccepting={isAccepting}
          isDeclining={isDeclining}
          onAccept={(fullName, password, birthDate) =>
            void acceptInvitation(fullName, password, birthDate)
          }
          onDecline={() => void declineInvitation()}
        />
      ) : null}
    </AuthenticationScreenLayout>
  );
}
