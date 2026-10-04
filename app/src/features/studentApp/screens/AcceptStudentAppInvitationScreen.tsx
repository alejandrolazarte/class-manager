import { useLocalSearchParams, useRouter } from "expo-router";
import { useEffect, useState } from "react";
import { isApiError, isNetworkError } from "@/api/httpClient";
import { getFieldErrors } from "@/api/problemDetails";
import { checkStudentAppInvitation } from "@/features/authentication/authenticationApi";
import { AuthenticationScreenLayout } from "@/features/authentication/components/AuthenticationScreenLayout";
import { useSession } from "@/features/authentication/useSession";
import { studentAppErrorCodes } from "@/features/studentApp/studentAppErrorCodes";
import { translate } from "@/i18n/translate";
import { routes } from "@/navigation/routes";
import { AppText } from "@/ui/AppText";
import { Banner } from "@/ui/Banner";
import { BrandMark } from "@/ui/BrandMark";
import { Button } from "@/ui/Button";
import { PasswordField } from "@/ui/PasswordField";
import { Spinner } from "@/ui/Spinner";
import { TextField } from "@/ui/TextField";

type AcceptStudentAppInvitationFailure = "invalidLink" | "alreadyLinked" | "network" | "unexpected";

interface FieldErrors {
  fullName?: string;
  password?: string;
}

const badRequestStatus = 400;

export function AcceptStudentAppInvitationScreen() {
  const router = useRouter();
  const { acceptStudentAppInvitation } = useSession();
  const { token } = useLocalSearchParams<{ token?: string }>();
  const [fullName, setFullName] = useState("");
  const [password, setPassword] = useState("");
  const [fieldErrors, setFieldErrors] = useState<FieldErrors>({});
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [failure, setFailure] = useState<AcceptStudentAppInvitationFailure | null>(
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
        await checkStudentAppInvitation({ token });
      } catch (checkError) {
        if (
          isCurrent &&
          isApiError(checkError) &&
          checkError.hasCode(studentAppErrorCodes.invalidInvitation)
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
    if (isApiError(acceptError) && acceptError.hasCode(studentAppErrorCodes.invalidInvitation)) {
      setFailure("invalidLink");
      return;
    }
    if (isApiError(acceptError) && acceptError.hasCode(studentAppErrorCodes.alreadyLinked)) {
      setFailure("alreadyLinked");
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
      await acceptStudentAppInvitation({ token, fullName: fullName.trim(), password });
      router.replace(routes.studentApp);
    } catch (acceptError) {
      handleAcceptError(acceptError);
    } finally {
      setIsSubmitting(false);
    }
  };

  const goToSignIn = () => router.replace(routes.signIn);

  return (
    <AuthenticationScreenLayout
      title={translate("studentAppInvitation.title")}
      onBack={goToSignIn}
      mark={<BrandMark />}
    >
      {failure === "invalidLink" ? (
        <Banner message={translate("studentAppInvitation.invalid")}>
          <Button
            variant="secondary"
            label={translate("studentAppInvitation.goToSignIn")}
            onPress={goToSignIn}
          />
        </Banner>
      ) : null}
      {failure === "alreadyLinked" ? (
        <Banner message={translate("studentAppInvitation.alreadyLinked")} />
      ) : null}
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
            {translate("studentAppInvitation.subtitle")}
          </AppText>
          <TextField
            label={translate("studentAppInvitation.fullName")}
            autoCapitalize="words"
            autoComplete="name"
            value={fullName}
            onChangeText={setFullName}
            errorMessage={fieldErrors.fullName}
          />
          <PasswordField
            label={translate("studentAppInvitation.password")}
            autoComplete="new-password"
            value={password}
            onChangeText={setPassword}
            onSubmitEditing={submit}
            errorMessage={fieldErrors.password}
          />
          <AppText variant="caption" tone="subtle">
            {translate("studentAppInvitation.passwordHint")}
          </AppText>
          <Button
            label={translate("studentAppInvitation.submit")}
            onPress={submit}
            isLoading={isSubmitting}
          />
        </>
      ) : null}
    </AuthenticationScreenLayout>
  );
}
