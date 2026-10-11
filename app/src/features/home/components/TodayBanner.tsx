import { startsInLabel, TodayHighlight } from "@/features/home/agenda";
import { DaySession } from "@/features/sessions/types";
import { translate } from "@/i18n/translate";
import { Highlight } from "@/ui/Highlight";

interface TodayBannerProps {
  highlight: TodayHighlight;
  timeNow: string;
  titleOf: (session: DaySession) => string;
  onOpen: (session: DaySession) => void;
}

export function TodayBanner({ highlight, timeNow, titleOf, onOpen }: TodayBannerProps) {
  if (highlight.kind === "live") {
    const { session } = highlight;
    return (
      <Highlight
        isLive
        eyebrow={translate("home.banner.live", { end: session.endTime })}
        title={titleOf(session)}
        actionLabel={translate("home.banner.attendance")}
        accessibilityLabel={translate("home.banner.attendance")}
        onPress={() => onOpen(session)}
      />
    );
  }

  if (highlight.kind === "next") {
    const { session } = highlight;
    const label = `${session.startTime} · ${titleOf(session)}`;
    return (
      <Highlight
        icon="schedule"
        eyebrow={`${translate("home.banner.next")} · ${startsInLabel(session.startTime, timeNow)}`}
        title={label}
        accessibilityLabel={`${translate("home.banner.next")}: ${label}`}
        onPress={() => onOpen(session)}
      />
    );
  }

  return (
    <Highlight
      tone="muted"
      icon="dayDone"
      eyebrow={translate("home.banner.next")}
      title={translate("home.banner.noneLeft")}
    />
  );
}
