import { View } from "react-native";
import { DaySession } from "@/features/sessions/types";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Card } from "@/ui/Card";
import { Icon } from "@/ui/Icon";
import { ProgressBar } from "@/ui/ProgressBar";

interface DaySessionCardProps {
  session: DaySession;
  onPress: (session: DaySession) => void;
}

const detailSeparator = " · ";
const studentNameSeparator = ", ";

export function DaySessionCard({ session, onPress }: DaySessionCardProps) {
  const isPrivate = session.kind === "Private";
  const title = isPrivate
    ? session.studentNames.join(studentNameSeparator)
    : session.classGroupName;
  const details = [session.instructorFullName, session.location].filter(
    (detail): detail is string => Boolean(detail),
  );
  return (
    <View className={`flex-row gap-3.5 ${session.isCancelled ? "opacity-60" : ""}`}>
      <View className="w-12 items-end gap-0.5 pt-3.5">
        <AppText variant="bodyStrong" className="font-heavy text-base">
          {session.startTime}
        </AppText>
        <AppText variant="footnote" tone="subtle">
          {session.endTime}
        </AppText>
      </View>
      <View className="min-w-0 flex-1">
        <Card
          onPress={() => onPress(session)}
          accessibilityLabel={`${session.startTime} ${title}`}
          className="gap-2.5 px-4 py-3.5"
        >
          <View className="flex-row items-start justify-between gap-2">
            <View className="min-w-0 flex-1 gap-[3px]">
              {isPrivate ? (
                <View className="flex-row items-center gap-1">
                  <Icon name="privateLesson" size="small" tone="primary" />
                  <AppText variant="badge" tone="primary">
                    {translate(session.isTrial ? "privateLessons.trial" : "privateLessons.kind")}
                  </AppText>
                </View>
              ) : null}
              <AppText variant="heading">{title}</AppText>
              <AppText variant="caption" tone="muted">
                {details.join(detailSeparator)}
              </AppText>
            </View>
            <Icon name="next" tone="subtle-foreground" />
          </View>
          {session.isCancelled ? (
            <View className="flex-row items-center gap-1.5">
              <Icon name="cancelled" size="small" tone="danger" />
              <AppText variant="label" tone="danger">
                {session.cancellationReason
                  ? translate("sessions.day.cancelledWithReason", {
                      reason: session.cancellationReason,
                    })
                  : translate("sessions.day.cancelled")}
              </AppText>
            </View>
          ) : (
            <View className="gap-1.5">
              <View className="flex-row">
                <ProgressBar
                  tone="success"
                  ratio={
                    session.enrolledCount > 0 ? session.presentCount / session.enrolledCount : 0
                  }
                />
              </View>
              <View className="flex-row justify-between gap-2">
                <AppText variant="label" tone="muted">
                  {translate("sessions.day.presentOfEnrolled", {
                    present: session.presentCount,
                    enrolled: session.enrolledCount,
                  })}
                </AppText>
                {session.originalStartTime ? (
                  <AppText variant="label" tone="primary">
                    {translate("sessions.day.rescheduled", {
                      original: session.originalStartTime,
                    })}
                  </AppText>
                ) : null}
              </View>
            </View>
          )}
        </Card>
      </View>
    </View>
  );
}
