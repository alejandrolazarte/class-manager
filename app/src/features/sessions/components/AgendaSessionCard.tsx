import { View } from "react-native";
import { SessionTiming, startsInLabel } from "@/features/home/agenda";
import { DaySession } from "@/features/sessions/types";
import { translate, translateCount, TranslationKey } from "@/i18n/translate";
import { AppText, TextTone } from "@/ui/AppText";
import { Card } from "@/ui/Card";

interface AgendaSessionCardProps {
  session: DaySession;
  title: string;
  timing: SessionTiming;
  timeNow: string;
  isToday: boolean;
  onPress: () => void;
}

type AgendaStatus = "cancelled" | "live" | "taken" | "pending" | "private" | "scheduled";

const metaSeparator = " · ";

const statusLabelKeys: Record<AgendaStatus, TranslationKey> = {
  cancelled: "home.status.cancelled",
  live: "home.status.live",
  taken: "home.status.taken",
  pending: "home.status.pending",
  private: "home.status.private",
  scheduled: "home.status.scheduled",
};

const statusPillClassNames: Record<AgendaStatus, string> = {
  cancelled: "bg-danger-soft",
  live: "bg-primary",
  taken: "bg-success-soft",
  pending: "bg-warning-soft",
  private: "bg-accent-soft",
  scheduled: "bg-primary-soft",
};

const statusTextTones: Record<AgendaStatus, TextTone> = {
  cancelled: "dangerSoft",
  live: "onPrimary",
  taken: "successSoft",
  pending: "warningSoft",
  private: "accentSoft",
  scheduled: "primarySoft",
};

const statusBarClassNames: Record<AgendaStatus, string> = {
  cancelled: "bg-disabled-foreground",
  live: "bg-primary",
  taken: "bg-success",
  pending: "bg-warning",
  private: "bg-accent",
  scheduled: "bg-primary",
};

function statusOf(session: DaySession, timing: SessionTiming): AgendaStatus {
  if (timing === "cancelled" || timing === "live") {
    return timing;
  }
  if (timing === "ended") {
    return session.presentCount + session.absentCount > 0 ? "taken" : "pending";
  }
  return session.kind === "Private" ? "private" : "scheduled";
}

function detailOf(
  session: DaySession,
  status: AgendaStatus,
  timeNow: string,
  isToday: boolean,
): string {
  if (status === "live") {
    return translate("home.detail.present", {
      present: session.presentCount,
      enrolled: session.enrolledCount,
    });
  }
  if (status === "taken") {
    return translate("home.detail.came", {
      present: session.presentCount,
      enrolled: session.enrolledCount,
    });
  }
  if ((status === "scheduled" || status === "private") && session.originalStartTime) {
    return translate("home.detail.rescheduled", { original: session.originalStartTime });
  }
  if ((status === "scheduled" || status === "private") && isToday) {
    return startsInLabel(session.startTime, timeNow);
  }
  return "";
}

export function AgendaSessionCard({
  session,
  title,
  timing,
  timeNow,
  isToday,
  onPress,
}: AgendaSessionCardProps) {
  const status = statusOf(session, timing);
  const meta = [
    session.kind === "Private"
      ? translate("home.meta.private")
      : translateCount("sessions.day.studentCount", session.enrolledCount),
    session.originalInstructorFullName
      ? translate("sessions.day.substitute", {
          substitute: session.instructorFullName,
          original: session.originalInstructorFullName,
        })
      : session.instructorFullName,
  ].join(metaSeparator);
  return (
    <Card
      onPress={onPress}
      accessibilityLabel={`${session.startTime} ${title}`}
      className={`h-[84px] flex-row items-center gap-3 px-4 py-3.5 ${status === "cancelled" ? "opacity-70" : ""}`}
    >
      <View className={`w-1 self-stretch rounded-sm ${statusBarClassNames[status]}`} />
      <View className="w-[52px]">
        <AppText variant="heading" className="font-heavy">
          {session.startTime}
        </AppText>
        <AppText variant="footnote" tone="subtle" className="font-label">
          {session.endTime}
        </AppText>
      </View>
      <View className="min-w-0 flex-1 gap-0.5">
        <AppText variant="bodyStrong" numberOfLines={1}>
          {title}
        </AppText>
        <AppText variant="caption" tone="subtle" numberOfLines={1}>
          {meta}
        </AppText>
      </View>
      <View className="items-end gap-1.5">
        <View className={`rounded-full px-2.5 py-[5px] ${statusPillClassNames[status]}`}>
          <AppText variant="badge" tone={statusTextTones[status]}>
            {translate(statusLabelKeys[status])}
          </AppText>
        </View>
        <AppText variant="footnote" tone="subtle" className="font-label">
          {detailOf(session, status, timeNow, isToday)}
        </AppText>
      </View>
    </Card>
  );
}
