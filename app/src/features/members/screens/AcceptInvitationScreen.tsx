import { useLocalSearchParams, useRouter } from "expo-router";
import { useEffect, useState } from "react";
import { getFieldErrors } from "@/api/problemDetails";
import { isApiError, isNetworkError } from "@/api/httpClient";
import { checkInvitation } from "@/features/authentication/authenticationApi";
import { AuthenticationScreenLayout } from "@/features/authentication/components/AuthenticationScreenLayout";
import { useSession } from "@/features/authentication/useSession";
import { memberErrorCodes } from "@/features/members/memberErrorCodes";
import { translate } from "@/i18n/translate";
import { routes } from "@/navigation/routes";
import { AppText } from "@/ui/AppText";
import { Banner } from "@/ui/Banner";
import { BrandMark } from "@/ui/BrandMark";
import { Button } from "@/ui/Button";
import { PasswordField } from "@/ui/PasswordField";
import { Spinner } from "@/ui/Spinner";
import { TextField } from "@/ui/TextField";

type AcceptInvitationFailure = "invalidLink" | "coachTaken" | "network" | "unexpected";

interface FieldErrors {
  fullName?: string;
  password?: string;
}

const badRequestStatus = 400;

export function AcceptInvitationScreen() {
  const router = useRouter();
  const { acceptInvitation } = useSession();
  const { token } = useLocalSearchParams<{ token?: string }>();
  const [fullName, setFullName] = useState("");
  const [password, setPassword] = useState("");
  const [fieldErrors, setFieldErrors] = useState<FieldErrors>({});
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [failure, setFailure] = useState<AcceptInvitationFailure | null>(
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
        await checkInvitation({ token });
      } catch (checkError) {
        if (
          isCurrent &&
          isApiError(checkError) &&
          checkError.hasCode(memberErrorCodes.invalidInvitation)
        ) {
          setFailure("invalidLink");
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

  const handleAcceptError = (acceptError: unknown) => {
    if (isNetworkError(acceptError)) {
      setFailure("network");
      return;
    }
    if (isApiError(acceptError) && acceptError.hasCode(memberErrorCodes.invalidInvitation)) {
      setFailure("invalidLink");
      return;
    }
    if (isApiError(acceptError) && acceptError.hasCode(memberErrorCodes.instructorTaken)) {
      setFailure("coachTaken");
      return;
    }
    if (isApiError(acceptError) && acceptError.status === badRequestStatus) {
      const serverFieldErrors = getFieldErrors(acceptError.problem);
      if (serverFieldErrors.fullName || serverFieldErrors.password) {
        setFieldErrors({
          fullName: serverFieldErrors.fullName,
          password: serverFieldErrors.password,
        });
        return;
      }
    }
    setFailure("unexpected");
  };

  const submit = async () => {
    if (!token) {
      return;
    }
    setFailure(null);
    setFieldErrors({});
    setIsSubmitting(true);
    try {
      await acceptInvitation({ token, fullName: fullName.trim(), password });
      router.replace(routes.today);
    } catch (acceptError) {
      handleAcceptError(acceptError);
    } finally {
      setIsSubmitting(false);
    }
  };

  const goToSignIn = () => router.replace(routes.signIn);

  return (
    <AuthenticationScreenLayout
      title={translate("invitation.title")}
      onBack={goToSignIn}
      mark={<BrandMark />}
    >
      {failure === "invalidLink" ? (
        <Banner message={translate("invitation.invalid")}>
          <Button
            variant="secondary"
            label={translate("invitation.goToSignIn")}
            onPress={goToSignIn}
          />
        </Banner>
      ) : null}
      {failure === "coachTaken" ? <Banner message={translate("invitation.coachTaken")} /> : null}
      {failure === "network" ? (
        <Banner message={translate("common.networkError")}>
          <Button variant="secondary" label={translate("common.retry")} onPress={submit} />
        </Banner>
      ) : null}
      {failure === "unexpected" ? <Banner message={translate("common.unexpectedError")} /> : null}
      {isChecking ? <Spinner className="my-4" /> : null}
      {token && !isChecking && failure !== "invalidLink" ? (
        <>
          <AppText variant="body" tone="muted">
            {translate("invitation.subtitle")}
          </AppText>
          <TextField
            label={translate("invitation.fullName")}
            autoCapitalize="words"
            autoComplete="name"
            value={fullName}
            onChangeText={setFullName}
            errorMessage={fieldErrors.fullName}
          />
          <PasswordField
            label={translate("invitation.password")}
            autoComplete="new-password"
            value={password}
            onChangeText={setPassword}
            onSubmitEditing={submit}
            errorMessage={fieldErrors.password}
          />
          <AppText variant="caption" tone="subtle">
            {translate("invitation.passwordHint")}
          </AppText>
          <Button
            label={translate("invitation.submit")}
            onPress={submit}
            isLoading={isSubmitting}
          />
        </>
      ) : null}
    </AuthenticationScreenLayout>
  );
}
