import { Pressable } from "react-native";
import { DaySession } from "@/features/sessions/types";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";

interface DaySessionCardProps {
  session: DaySession;
  onPress: (session: DaySession) => void;
}

const detailSeparator = " · ";

export function DaySessionCard({ session, onPress }: DaySessionCardProps) {
  const status = session.isCancelled
    ? session.cancellationReason
      ? translate("sessions.day.cancelledWithReason", { reason: session.cancellationReason })
      : translate("sessions.day.cancelled")
    : translate("sessions.day.presentOfEnrolled", {
        present: session.presentCount,
        enrolled: session.enrolledCount,
      });
  return (
    <Pressable
      accessibilityRole="button"
      accessibilityLabel={`${session.startTime} ${session.classGroupName}`}
      onPress={() => onPress(session)}
      className={`gap-1 border-b border-border-subtle bg-surface px-4 py-3 ${session.isCancelled ? "opacity-60" : ""}`}
    >
      <AppText variant="label" tone="primary">
        {session.originalStartTime
          ? `${session.startTime}–${session.endTime} · ${translate("sessions.day.rescheduled", { original: session.originalStartTime })}`
          : `${session.startTime}–${session.endTime}`}
      </AppText>
      <AppText variant="bodyStrong">{session.classGroupName}</AppText>
      <AppText variant="caption" tone={session.isCancelled ? "danger" : "muted"}>
        {[session.instructorFullName, status].join(detailSeparator)}
      </AppText>
    </Pressable>
  );
}
