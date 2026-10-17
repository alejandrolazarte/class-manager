import { useQueryClient } from "@tanstack/react-query";
import { useRouter } from "expo-router";
import { useState } from "react";
import { isApiError, isNetworkError } from "@/api/httpClient";
import { getFieldErrors } from "@/api/problemDetails";
import { instructorErrorCodes } from "@/features/instructors/instructorErrorCodes";
import { SeparatedRole, separatePerson } from "@/features/users/separationApi";
import { emailAddressPattern } from "@/forms/emailAddress";
import { isFilled } from "@/forms/requiredFields";
import { TranslationKey, translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";
import { ConfirmDialog } from "@/ui/ConfirmDialog";
import { RequiredFieldsLegend } from "@/ui/RequiredFieldsLegend";
import { ScreenFooter, ScrollScreen } from "@/ui/Screen";
import { ScreenHeader } from "@/ui/ScreenHeader";
import { TextField } from "@/ui/TextField";
import { useToast } from "@/ui/ToastProvider";

const badRequestStatus = 400;

const consequenceKeys: Record<SeparatedRole, TranslationKey> = {
  instructor: "people.separate.consequence.instructor",
  client: "people.separate.consequence.client",
  student: "people.separate.consequence.student",
};

type SeparationFailure = "network" | "onTheTeam" | "unexpected";

interface SeparatePersonScreenProps {
  role: SeparatedRole;
  id: string;
  currentFullName: string;
}

export function SeparatePersonScreen({ role, id, currentFullName }: SeparatePersonScreenProps) {
  const router = useRouter();
  const queryClient = useQueryClient();
  const { showToast } = useToast();
  const [fullName, setFullName] = useState("");
  const [email, setEmail] = useState("");
  const [fullNameError, setFullNameError] = useState<string | undefined>();
  const [emailError, setEmailError] = useState<string | undefined>();
  const [failure, setFailure] = useState<SeparationFailure | null>(null);
  const [isConfirming, setIsConfirming] = useState(false);
  const [isSeparating, setIsSeparating] = useState(false);

  const askToConfirm = () => {
    setFailure(null);
    setFullNameError(undefined);
    const trimmedEmail = email.trim();
    if (trimmedEmail.length > 0 && !emailAddressPattern.test(trimmedEmail)) {
      setEmailError(translate("people.separate.emailInvalid"));
      return;
    }
    setEmailError(undefined);
    setIsConfirming(true);
  };

  const handleSeparationError = (separationError: unknown) => {
    if (isNetworkError(separationError)) {
      setFailure("network");
      return;
    }
    if (isApiError(separationError) && separationError.hasCode(instructorErrorCodes.onTheTeam)) {
      setFailure("onTheTeam");
      return;
    }
    if (isApiError(separationError) && separationError.status === badRequestStatus) {
      const fieldErrors = getFieldErrors(separationError.problem);
      setFullNameError(fieldErrors.fullName);
      setEmailError(fieldErrors.email);
      if (fieldErrors.fullName || fieldErrors.email) {
        return;
      }
    }
    setFailure("unexpected");
  };

  const separate = async () => {
    const trimmedFullName = fullName.trim();
    const trimmedEmail = email.trim();
    setIsSeparating(true);
    try {
      await separatePerson(role, id, {
        fullName: trimmedFullName,
        email: trimmedEmail.length > 0 ? trimmedEmail : null,
      });
      await queryClient.invalidateQueries();
      showToast(translate("people.separate.done", { name: trimmedFullName }));
      router.back();
    } catch (separationError) {
      handleSeparationError(separationError);
    } finally {
      setIsConfirming(false);
      setIsSeparating(false);
    }
  };

  return (
    <ScrollScreen
      header={<ScreenHeader navigation="close" title={translate("people.separate.title")} />}
      footer={
        <ScreenFooter>
          <Button
            variant="dangerOutline"
            label={translate("people.separate.submit")}
            onPress={askToConfirm}
            disabled={!isFilled(fullName)}
          />
        </ScreenFooter>
      }
    >
      <AppText variant="body" tone="subtle">
        {translate("people.separate.hint", { name: currentFullName })}
      </AppText>
      {failure === "network" ? (
        <Banner message={translate("common.networkError")}>
          <Button variant="secondary" label={translate("common.retry")} onPress={askToConfirm} />
        </Banner>
      ) : null}
      {failure === "onTheTeam" ? (
        <Banner tone="warning" message={translate("people.separate.onTheTeam")} />
      ) : null}
      {failure === "unexpected" ? <Banner message={translate("common.unexpectedError")} /> : null}
      <RequiredFieldsLegend />
      <TextField
        label={translate("people.separate.fullName")}
        isRequired
        autoCapitalize="words"
        autoComplete="name"
        value={fullName}
        onChangeText={setFullName}
        errorMessage={fullNameError}
      />
      <TextField
        label={translate("people.separate.email")}
        hint={translate("people.separate.emailHint", { name: currentFullName })}
        keyboardType="email-address"
        autoCapitalize="none"
        autoComplete="email"
        value={email}
        onChangeText={setEmail}
        errorMessage={emailError}
      />
      {isConfirming ? (
        <ConfirmDialog
          title={translate("people.separate.question", {
            name: fullName.trim(),
            currentName: currentFullName,
          })}
          message={translate(consequenceKeys[role], {
            name: fullName.trim(),
            currentName: currentFullName,
          })}
          confirmLabel={translate("people.separate.confirm")}
          onCancel={() => setIsConfirming(false)}
          onConfirm={() => void separate()}
          isConfirming={isSeparating}
        />
      ) : null}
    </ScrollScreen>
  );
}
