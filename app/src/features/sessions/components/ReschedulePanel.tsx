import { useState } from "react";
import { View } from "react-native";
import { isApiError } from "@/api/httpClient";
import { startTimePattern } from "@/features/classGroups/startTimeFormatting";
import { sessionErrorCodes } from "@/features/sessions/sessionErrorCodes";
import { useRescheduleSession } from "@/features/sessions/useSessionMutations";
import { translate } from "@/i18n/translate";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";
import { ListRow } from "@/ui/ListRow";
import { useToast } from "@/ui/ToastProvider";
import { isFilled } from "@/forms/requiredFields";
import { TimeField } from "@/forms/TimeField";

interface ReschedulePanelProps {
  classGroupId: string;
  sessionDate: string;
  currentStartTime: string;
}

type RescheduleFailure = "instructorBusy" | "invalidTime" | "unexpected";

const badRequestStatus = 400;

const failureMessages = {
  instructorBusy: "sessions.reschedule.instructorBusy",
  invalidTime: "sessions.reschedule.invalidTime",
  unexpected: "common.unexpectedError",
} as const;

export function ReschedulePanel({
  classGroupId,
  sessionDate,
  currentStartTime,
}: ReschedulePanelProps) {
  const { showToast } = useToast();
  const rescheduleSessionMutation = useRescheduleSession(classGroupId, sessionDate);
  const [isOpen, setIsOpen] = useState(false);
  const [startTime, setStartTime] = useState("");
  const [failure, setFailure] = useState<RescheduleFailure | null>(null);

  const toggle = () => {
    setFailure(null);
    setStartTime(isOpen ? "" : currentStartTime);
    setIsOpen(!isOpen);
  };

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
      showToast(translate("sessions.reschedule.saved"));
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

  return (
    <View>
      <ListRow
        icon="schedule"
        label={translate("sessions.reschedule.open")}
        isExpanded={isOpen}
        onPress={toggle}
      />
      {isOpen ? (
        <View className="gap-2.5 px-4 pb-4">
          {failure ? <Banner tone="warning" message={translate(failureMessages[failure])} /> : null}
          <TimeField
            label={translate("sessions.reschedule.startTime")}
            isRequired
            fieldSurface="background"
            value={startTime}
            onChangeText={setStartTime}
          />
          <Button
            size="medium"
            label={translate("sessions.reschedule.confirm")}
            onPress={confirm}
            disabled={!isFilled(startTime)}
            isLoading={rescheduleSessionMutation.isPending}
          />
        </View>
      ) : null}
    </View>
  );
}
