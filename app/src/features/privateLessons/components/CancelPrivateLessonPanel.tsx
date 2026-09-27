import { useState } from "react";
import { View } from "react-native";
import { useCancelPrivateLesson } from "@/features/privateLessons/usePrivateLessonMutations";
import { translate } from "@/i18n/translate";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";
import { ListRow } from "@/ui/ListRow";
import { TextField } from "@/ui/TextField";
import { useToast } from "@/ui/ToastProvider";

interface CancelPrivateLessonPanelProps {
  privateLessonId: string;
}

export function CancelPrivateLessonPanel({ privateLessonId }: CancelPrivateLessonPanelProps) {
  const { showToast } = useToast();
  const cancelMutation = useCancelPrivateLesson(privateLessonId);
  const [isOpen, setIsOpen] = useState(false);
  const [reason, setReason] = useState("");
  const [hasFailed, setHasFailed] = useState(false);

  const confirm = async () => {
    setHasFailed(false);
    const trimmedReason = reason.trim();
    try {
      await cancelMutation.mutateAsync(trimmedReason.length > 0 ? trimmedReason : null);
      setIsOpen(false);
      setReason("");
      showToast(translate("sessions.session.cancelledToast"));
    } catch {
      setHasFailed(true);
    }
  };

  return (
    <View>
      <ListRow
        icon="cancelled"
        iconTone="danger"
        label={translate("privateLessons.cancel")}
        isExpanded={isOpen}
        onPress={() => setIsOpen(!isOpen)}
      />
      {isOpen ? (
        <View className="gap-2.5 px-4 pb-4">
          {hasFailed ? <Banner message={translate("common.unexpectedError")} /> : null}
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
            isLoading={cancelMutation.isPending}
          />
        </View>
      ) : null}
    </View>
  );
}
