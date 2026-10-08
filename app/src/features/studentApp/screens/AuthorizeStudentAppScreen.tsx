import { useLocalSearchParams, useRouter } from "expo-router";
import { useEffect, useState } from "react";
import { isNetworkError } from "@/api/httpClient";
import {
  checkGuardianConsent,
  giveGuardianConsent,
  refuseGuardianConsent,
} from "@/features/authentication/authenticationApi";
import { AuthenticationScreenLayout } from "@/features/authentication/components/AuthenticationScreenLayout";
import { GuardianConsentRequest } from "@/features/authentication/types";
import { translate } from "@/i18n/translate";
import { routes } from "@/navigation/routes";
import { AppText } from "@/ui/AppText";
import { Banner } from "@/ui/Banner";
import { BrandMark } from "@/ui/BrandMark";
import { Button } from "@/ui/Button";
import { Spinner } from "@/ui/Spinner";

type GuardianAnswer = "authorized" | "refused";

type GuardianConsentFailure = "invalidLink" | "network" | "unexpected";

export function AuthorizeStudentAppScreen() {
  const router = useRouter();
  const { token } = useLocalSearchParams<{ token?: string }>();
  const [request, setRequest] = useState<GuardianConsentRequest | null>(null);
  const [answer, setAnswer] = useState<GuardianAnswer | null>(null);
  const [pendingAnswer, setPendingAnswer] = useState<GuardianAnswer | null>(null);
  const [failure, setFailure] = useState<GuardianConsentFailure | null>(
    token ? null : "invalidLink",
  );
  const [isChecking, setIsChecking] = useState(Boolean(token));

  useEffect(() => {
    if (!token) {
      return;
    }
    let isCurrent = true;
    const verifyLink = async () => {
      try {
        const checkedRequest = await checkGuardianConsent({ token });
        if (isCurrent) {
          setRequest(checkedRequest);
        }
      } catch (checkError) {
        if (isCurrent) {
          setFailure(isNetworkError(checkError) ? "network" : "invalidLink");
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
  }, [token]);

  const respond = async (guardianAnswer: GuardianAnswer) => {
    if (!token) {
      return;
    }
    setFailure(null);
    setPendingAnswer(guardianAnswer);
    try {
      await (guardianAnswer === "authorized"
        ? giveGuardianConsent({ token })
        : refuseGuardianConsent({ token }));
      setAnswer(guardianAnswer);
    } catch (answerError) {
      setFailure(isNetworkError(answerError) ? "network" : "unexpected");
    } finally {
      setPendingAnswer(null);
    }
  };

  const goToSignIn = () => router.replace(routes.signIn);

  return (
    <AuthenticationScreenLayout
      title={translate("guardianConsent.title")}
      onBack={goToSignIn}
      mark={<BrandMark />}
    >
      {failure === "invalidLink" ? (
        <Banner message={translate("guardianConsent.invalid")}>
          <Button
            variant="secondary"
            label={translate("invitation.goToSignIn")}
            onPress={goToSignIn}
          />
        </Banner>
      ) : null}
      {failure === "network" ? <Banner message={translate("common.networkError")} /> : null}
      {failure === "unexpected" ? <Banner message={translate("common.unexpectedError")} /> : null}
      {isChecking ? <Spinner className="my-4" /> : null}
      {request && answer === "authorized" ? (
        <Banner
          tone="success"
          message={translate("guardianConsent.authorized", { email: request.studentEmail })}
        />
      ) : null}
      {request && answer === "refused" ? (
        <Banner tone="info" message={translate("guardianConsent.refused")} />
      ) : null}
      {request && answer === null && failure !== "invalidLink" ? (
        <>
          <AppText variant="body">
            {translate("guardianConsent.request", {
              business: request.businessName,
              name: request.studentFullName,
              age: request.minimumAge,
            })}
          </AppText>
          <AppText variant="body" tone="muted">
            {translate("guardianConsent.details", {
              name: request.studentFullName,
              email: request.studentEmail,
            })}
          </AppText>
          <Button
            label={translate("guardianConsent.authorize")}
            onPress={() => void respond("authorized")}
            disabled={pendingAnswer === "refused"}
            isLoading={pendingAnswer === "authorized"}
          />
          <Button
            variant="outline"
            label={translate("guardianConsent.refuse")}
            onPress={() => void respond("refused")}
            disabled={pendingAnswer === "authorized"}
            isLoading={pendingAnswer === "refused"}
          />
        </>
      ) : null}
    </AuthenticationScreenLayout>
  );
}
