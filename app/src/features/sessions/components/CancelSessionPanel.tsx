import { useState } from "react";
import { View } from "react-native";
import { isApiError } from "@/api/httpClient";
import { sessionErrorCodes } from "@/features/sessions/sessionErrorCodes";
import { useCancelSession } from "@/features/sessions/useSessionMutations";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";
import { ListRow } from "@/ui/ListRow";
import { TextField } from "@/ui/TextField";
import { useToast } from "@/ui/ToastProvider";

interface CancelSessionPanelProps {
  classGroupId: string;
  sessionDate: string;
  hasAttendance: boolean;
}

type CancelFailure = "hasAttendance" | "unexpected";

export function CancelSessionPanel({
  classGroupId,
  sessionDate,
  hasAttendance,
}: CancelSessionPanelProps) {
  const { showToast } = useToast();
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
      showToast(translate("sessions.session.cancelledToast"));
    } catch (cancelError) {
      setCancelFailure(
        isApiError(cancelError) && cancelError.hasCode(sessionErrorCodes.hasAttendance)
          ? "hasAttendance"
          : "unexpected",
      );
    }
  };

  return (
    <View>
      <ListRow
        icon="cancelled"
        iconTone="danger"
        label={translate("sessions.session.cancel")}
        isExpanded={isOpen}
        onPress={() => {
          setCancelFailure(null);
          setIsOpen(!isOpen);
        }}
      />
      {isOpen ? (
        <View className="gap-2.5 px-4 pb-4">
          {hasAttendance ? (
            <AppText variant="label" tone="warning">
              {translate("sessions.session.cancelHasAttendance")}
            </AppText>
          ) : (
            <>
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
                fieldSurface="background"
                value={reason}
                onChangeText={setReason}
              />
              <Button
                size="medium"
                variant="danger"
                label={translate("sessions.session.confirmCancel")}
                onPress={confirm}
                isLoading={cancelSessionMutation.isPending}
              />
            </>
          )}
        </View>
      ) : null}
    </View>
  );
}
