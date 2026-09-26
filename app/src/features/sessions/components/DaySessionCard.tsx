import { Pressable, Text } from "react-native";
import { DaySession } from "@/features/sessions/types";
import { translate } from "@/i18n/translate";

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
      className={`gap-1 border-b border-gray-100 bg-white px-4 py-3 ${session.isCancelled ? "opacity-60" : ""}`}
    >
      <Text className="text-sm font-medium text-brand">
        {session.originalStartTime
          ? `${session.startTime}–${session.endTime} · ${translate("sessions.day.rescheduled", { original: session.originalStartTime })}`
          : `${session.startTime}–${session.endTime}`}
      </Text>
      <Text className="text-base font-semibold text-gray-900">{session.classGroupName}</Text>
      <Text className={`text-sm ${session.isCancelled ? "text-red-600" : "text-gray-600"}`}>
        {[session.instructorFullName, status].join(detailSeparator)}
      </Text>
    </Pressable>
  );
}
