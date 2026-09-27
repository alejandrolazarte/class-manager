import { useState } from "react";
import { View } from "react-native";
import { isApiError } from "@/api/httpClient";
import { sessionErrorCodes } from "@/features/sessions/sessionErrorCodes";
import { useCancelSession } from "@/features/sessions/useSessionMutations";
import { translate } from "@/i18n/translate";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";
import { TextField } from "@/ui/TextField";

interface CancelSessionPanelProps {
  classGroupId: string;
  sessionDate: string;
}

type CancelFailure = "hasAttendance" | "unexpected";

export function CancelSessionPanel({ classGroupId, sessionDate }: CancelSessionPanelProps) {
  const cancelSessionMutation = useCancelSession(classGroupId, sessionDate);
  const [isOpen, setIsOpen] = useState(false);
  const [reason, setReason] = useState("");
  const [cancelFailure, setCancelFailure] = useState<CancelFailure | null>(null);

  const confirm = async () => {
    setCancelFailure(null);
    const trimmedReason = reason.trim();
    try {
      await cancelSessionMutation.mutateAsync(trimmedReason.length > 0 ? trimmedReason : null);
      setIsOpen(false);
      setReason("");
    } catch (cancelError) {
      setCancelFailure(
        isApiError(cancelError) && cancelError.hasCode(sessionErrorCodes.hasAttendance)
          ? "hasAttendance"
          : "unexpected",
      );
    }
  };

  if (!isOpen) {
    return (
      <Button
        variant="secondary"
        label={translate("sessions.session.cancel")}
        onPress={() => setIsOpen(true)}
      />
    );
  }

  return (
    <View className="gap-3 rounded-xl border border-border bg-surface p-3">
      {cancelFailure ? (
        <Banner
          tone="warning"
          message={translate(
            cancelFailure === "hasAttendance"
              ? "sessions.session.cancelHasAttendance"
              : "common.unexpectedError",
          )}
        />
      ) : null}
      <TextField
        label={translate("sessions.session.cancelReason")}
        placeholder={translate("sessions.session.cancelReasonPlaceholder")}
        value={reason}
        onChangeText={setReason}
      />
      <Button
        variant="danger"
        label={translate("sessions.session.confirmCancel")}
        onPress={confirm}
        isLoading={cancelSessionMutation.isPending}
      />
      <Button
        variant="secondary"
        label={translate("common.cancel")}
        onPress={() => setIsOpen(false)}
      />
    </View>
  );
}
