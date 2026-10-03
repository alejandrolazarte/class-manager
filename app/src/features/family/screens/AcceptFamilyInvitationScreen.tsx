import { useLocalSearchParams, useRouter } from "expo-router";
import { useEffect, useState } from "react";
import { isApiError, isNetworkError } from "@/api/httpClient";
import { getFieldErrors } from "@/api/problemDetails";
import { checkFamilyInvitation } from "@/features/authentication/authenticationApi";
import { AuthenticationScreenLayout } from "@/features/authentication/components/AuthenticationScreenLayout";
import { useSession } from "@/features/authentication/useSession";
import { familyErrorCodes } from "@/features/family/familyErrorCodes";
import { translate } from "@/i18n/translate";
import { routes } from "@/navigation/routes";
import { AppText } from "@/ui/AppText";
import { Banner } from "@/ui/Banner";
import { BrandMark } from "@/ui/BrandMark";
import { Button } from "@/ui/Button";
import { PasswordField } from "@/ui/PasswordField";
import { Spinner } from "@/ui/Spinner";
import { TextField } from "@/ui/TextField";

type AcceptFamilyInvitationFailure = "invalidLink" | "alreadyLinked" | "network" | "unexpected";

interface FieldErrors {
  fullName?: string;
  password?: string;
}

const badRequestStatus = 400;

export function AcceptFamilyInvitationScreen() {
  const router = useRouter();
  const { acceptFamilyInvitation } = useSession();
  const { token } = useLocalSearchParams<{ token?: string }>();
  const [fullName, setFullName] = useState("");
  const [password, setPassword] = useState("");
  const [fieldErrors, setFieldErrors] = useState<FieldErrors>({});
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [failure, setFailure] = useState<AcceptFamilyInvitationFailure | null>(
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
        await checkFamilyInvitation({ token });
      } catch (checkError) {
        if (
          isCurrent &&
          isApiError(checkError) &&
          checkError.hasCode(familyErrorCodes.invalidInvitation)
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
    if (isApiError(acceptError) && acceptError.hasCode(familyErrorCodes.invalidInvitation)) {
      setFailure("invalidLink");
      return;
    }
    if (isApiError(acceptError) && acceptError.hasCode(familyErrorCodes.alreadyLinked)) {
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
      await acceptFamilyInvitation({ token, fullName: fullName.trim(), password });
      router.replace(routes.family);
    } catch (acceptError) {
      handleAcceptError(acceptError);
    } finally {
      setIsSubmitting(false);
    }
  };

  const goToSignIn = () => router.replace(routes.signIn);

  return (
    <AuthenticationScreenLayout
      title={translate("familyInvitation.title")}
      onBack={goToSignIn}
      mark={<BrandMark />}
    >
      {failure === "invalidLink" ? (
        <Banner message={translate("familyInvitation.invalid")}>
          <Button
            variant="secondary"
            label={translate("familyInvitation.goToSignIn")}
            onPress={goToSignIn}
          />
        </Banner>
      ) : null}
      {failure === "alreadyLinked" ? (
        <Banner message={translate("familyInvitation.alreadyLinked")} />
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
            {translate("familyInvitation.subtitle")}
          </AppText>
          <TextField
            label={translate("familyInvitation.fullName")}
            autoCapitalize="words"
            autoComplete="name"
            value={fullName}
            onChangeText={setFullName}
            errorMessage={fieldErrors.fullName}
          />
          <PasswordField
            label={translate("familyInvitation.password")}
            autoComplete="new-password"
            value={password}
            onChangeText={setPassword}
            onSubmitEditing={submit}
            errorMessage={fieldErrors.password}
          />
          <AppText variant="caption" tone="subtle">
            {translate("familyInvitation.passwordHint")}
          </AppText>
          <Button
            label={translate("familyInvitation.submit")}
            onPress={submit}
            isLoading={isSubmitting}
          />
        </>
      ) : null}
    </AuthenticationScreenLayout>
  );
}
