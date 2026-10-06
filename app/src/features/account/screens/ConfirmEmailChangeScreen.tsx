import { useQueryClient } from "@tanstack/react-query";
import { useLocalSearchParams, useRouter } from "expo-router";
import { useCallback, useEffect, useRef, useState } from "react";
import { isApiError, isNetworkError } from "@/api/httpClient";
import { confirmEmailChange } from "@/features/account/accountApi";
import { accountErrorCodes } from "@/features/account/accountErrorCodes";
import { accountQueryKeys } from "@/features/account/accountQueryKeys";
import { AuthenticationScreenLayout } from "@/features/authentication/components/AuthenticationScreenLayout";
import { translate } from "@/i18n/translate";
import { routes } from "@/navigation/routes";
import { Banner } from "@/ui/Banner";
import { BrandMark } from "@/ui/BrandMark";
import { Button } from "@/ui/Button";
import { Spinner } from "@/ui/Spinner";

type Confirmation =
  | { state: "confirming" }
  | { state: "confirmed"; newEmail: string }
  | { state: "invalidLink" }
  | { state: "emailTaken" }
  | { state: "network" }
  | { state: "unexpected" };

export function ConfirmEmailChangeScreen() {
  const router = useRouter();
  const queryClient = useQueryClient();
  const { token } = useLocalSearchParams<{ token?: string }>();
  const [confirmation, setConfirmation] = useState<Confirmation>(
    token ? { state: "confirming" } : { state: "invalidLink" },
  );
  const hasStarted = useRef(false);

  const confirm = useCallback(async () => {
    if (!token) {
      return;
    }
    setConfirmation({ state: "confirming" });
    try {
      const { newEmail } = await confirmEmailChange(token);
      await queryClient.invalidateQueries({ queryKey: accountQueryKeys.myAccount });
      setConfirmation({ state: "confirmed", newEmail });
    } catch (confirmationError) {
      if (isNetworkError(confirmationError)) {
        setConfirmation({ state: "network" });
      } else if (
        isApiError(confirmationError) &&
        confirmationError.hasCode(accountErrorCodes.emailTaken)
      ) {
        setConfirmation({ state: "emailTaken" });
      } else if (
        isApiError(confirmationError) &&
        confirmationError.hasCode(accountErrorCodes.invalidEmailChangeToken)
      ) {
        setConfirmation({ state: "invalidLink" });
      } else {
        setConfirmation({ state: "unexpected" });
      }
    }
  }, [queryClient, token]);

  useEffect(() => {
    if (hasStarted.current) {
      return;
    }
    hasStarted.current = true;
    void confirm();
  }, [confirm]);

  const goToApp = () => router.replace(routes.appEntry);

  return (
    <AuthenticationScreenLayout
      title={translate("profile.confirm.title")}
      onBack={goToApp}
      mark={<BrandMark />}
    >
      {confirmation.state === "confirming" ? <Spinner className="mt-6" /> : null}
      {confirmation.state === "confirmed" ? (
        <Banner
          tone="success"
          message={translate("profile.confirm.done", { email: confirmation.newEmail })}
        >
          <Button
            variant="secondary"
            label={translate("profile.confirm.goToApp")}
            onPress={goToApp}
          />
        </Banner>
      ) : null}
      {confirmation.state === "invalidLink" ? (
        <Banner message={translate("profile.confirm.invalidLink")} />
      ) : null}
      {confirmation.state === "emailTaken" ? (
        <Banner message={translate("profile.email.taken")} />
      ) : null}
      {confirmation.state === "network" ? (
        <Banner message={translate("common.networkError")}>
          <Button variant="secondary" label={translate("common.retry")} onPress={confirm} />
        </Banner>
      ) : null}
      {confirmation.state === "unexpected" ? (
        <Banner message={translate("common.unexpectedError")} />
      ) : null}
    </AuthenticationScreenLayout>
  );
}
