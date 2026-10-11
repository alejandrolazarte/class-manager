import { SessionTiming, startsInLabel } from "@/features/home/agenda";
import { DaySession } from "@/features/sessions/types";
import { translate, translateCount, TranslationKey } from "@/i18n/translate";
import { TimelineRow, TimelineTone } from "@/ui/TimelineRow";

interface AgendaSessionRowProps {
  session: DaySession;
  title: string;
  timing: SessionTiming;
  timeNow: string;
  isToday: boolean;
  isFirst: boolean;
  isLast: boolean;
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

const statusTones: Record<AgendaStatus, TimelineTone> = {
  cancelled: "cancelled",
  live: "live",
  taken: "done",
  pending: "pending",
  private: "upcoming",
  scheduled: "upcoming",
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

export function AgendaSessionRow({
  session,
  title,
  timing,
  timeNow,
  isToday,
  isFirst,
  isLast,
  onPress,
}: AgendaSessionRowProps) {
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
    <TimelineRow
      startTime={session.startTime}
      endTime={session.endTime}
      title={title}
      meta={meta}
      status={translate(statusLabelKeys[status])}
      detail={detailOf(session, status, timeNow, isToday)}
      tone={statusTones[status]}
      isFirst={isFirst}
      isLast={isLast}
      onPress={onPress}
      accessibilityLabel={`${session.startTime} ${title}`}
    />
  );
}
