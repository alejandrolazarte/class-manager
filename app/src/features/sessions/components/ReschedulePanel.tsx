import { useState } from "react";
import { View } from "react-native";
import { isApiError } from "@/api/httpClient";
import {
  formatStartTimeAsTyped,
  startTimePattern,
} from "@/features/classGroups/startTimeFormatting";
import { sessionErrorCodes } from "@/features/sessions/sessionErrorCodes";
import { useRescheduleSession } from "@/features/sessions/useSessionMutations";
import { translate } from "@/i18n/translate";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";
import { TextField } from "@/ui/TextField";

interface ReschedulePanelProps {
  classGroupId: string;
  sessionDate: string;
}

type RescheduleFailure = "instructorBusy" | "invalidTime" | "unexpected";

const badRequestStatus = 400;

const failureMessages = {
  instructorBusy: "sessions.reschedule.instructorBusy",
  invalidTime: "sessions.reschedule.invalidTime",
  unexpected: "common.unexpectedError",
} as const;

export function ReschedulePanel({ classGroupId, sessionDate }: ReschedulePanelProps) {
  const rescheduleSessionMutation = useRescheduleSession(classGroupId, sessionDate);
  const [isOpen, setIsOpen] = useState(false);
  const [startTime, setStartTime] = useState("");
  const [failure, setFailure] = useState<RescheduleFailure | null>(null);

  const confirm = async () => {
    setFailure(null);
    if (!startTimePattern.test(startTime)) {
      setFailure("invalidTime");
      return;
    }
    try {
      await rescheduleSessionMutation.mutateAsync(startTime);
      setIsOpen(false);
      setStartTime("");
    } catch (rescheduleError) {
      if (
        isApiError(rescheduleError) &&
        rescheduleError.hasCode(sessionErrorCodes.instructorBusy)
      ) {
        setFailure("instructorBusy");
        return;
      }
      setFailure(
        isApiError(rescheduleError) && rescheduleError.status === badRequestStatus
          ? "invalidTime"
          : "unexpected",
      );
    }
  };

  if (!isOpen) {
    return (
      <Button
        variant="secondary"
        label={translate("sessions.reschedule.open")}
        onPress={() => setIsOpen(true)}
      />
    );
  }

  return (
    <View className="gap-3 rounded-xl border border-border bg-surface p-3">
      {failure ? <Banner tone="warning" message={translate(failureMessages[failure])} /> : null}
      <TextField
        label={translate("sessions.reschedule.startTime")}
        placeholder={translate("classGroups.form.startTimePlaceholder")}
        keyboardType="number-pad"
        value={startTime}
        onChangeText={(typedText) => setStartTime(formatStartTimeAsTyped(typedText))}
      />
      <Button
        label={translate("sessions.reschedule.confirm")}
        onPress={confirm}
        isLoading={rescheduleSessionMutation.isPending}
      />
      <Button
        variant="secondary"
        label={translate("common.cancel")}
        onPress={() => setIsOpen(false)}
      />
    </View>
  );
}
