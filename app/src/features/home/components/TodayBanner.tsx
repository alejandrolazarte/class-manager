import { Pressable, View } from "react-native";
import { startsInLabel, TodayHighlight } from "@/features/home/agenda";
import { DaySession } from "@/features/sessions/types";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Icon } from "@/ui/Icon";
import { useElevationStyle } from "@/ui/elevation";

interface TodayBannerProps {
  highlight: TodayHighlight;
  timeNow: string;
  titleOf: (session: DaySession) => string;
  onOpen: (session: DaySession) => void;
}

const bannerClassName = "h-[72px] flex-row items-center gap-3 rounded-[20px] px-4";

export function TodayBanner({ highlight, timeNow, titleOf, onOpen }: TodayBannerProps) {
  const cardStyle = useElevationStyle();
  const floatingStyle = useElevationStyle("floating");

  if (highlight.kind === "live") {
    const { session } = highlight;
    return (
      <View style={floatingStyle} className={`${bannerClassName} bg-primary`}>
        <View className="h-10 w-10 items-center justify-center rounded-xl bg-primary-foreground/20">
          <Icon name="live" tone="primary-foreground" />
        </View>
        <View className="min-w-0 flex-1">
          <AppText variant="overline" tone="onPrimary" className="opacity-85">
            {translate("home.banner.live", { end: session.endTime })}
          </AppText>
          <AppText variant="bodyStrong" tone="onPrimary" numberOfLines={1}>
            {titleOf(session)}
          </AppText>
        </View>
        <Pressable
          accessibilityRole="button"
          accessibilityLabel={translate("home.banner.attendance")}
          onPress={() => onOpen(session)}
          className="h-9 flex-row items-center gap-1 rounded-xl bg-surface px-3 active:opacity-80"
        >
          <Icon name="attendance" size="medium" tone="primary-strong" />
          <AppText variant="bodyStrong" tone="primary">
            {translate("home.banner.attendance")}
          </AppText>
        </Pressable>
      </View>
    );
  }

  if (highlight.kind === "next") {
    const { session } = highlight;
    const label = `${session.startTime} · ${titleOf(session)}`;
    return (
      <Pressable
        accessibilityRole="button"
        accessibilityLabel={`${translate("home.banner.next")}: ${label}`}
        onPress={() => onOpen(session)}
        style={cardStyle}
        className={`${bannerClassName} bg-surface active:bg-muted`}
      >
        <View className="h-10 w-10 items-center justify-center rounded-xl bg-primary-soft">
          <Icon name="schedule" tone="primary-soft-foreground" />
        </View>
        <View className="min-w-0 flex-1">
          <AppText variant="overline" tone="subtle">
            {`${translate("home.banner.next")} · ${startsInLabel(session.startTime, timeNow)}`}
          </AppText>
          <AppText variant="bodyStrong" numberOfLines={1}>
            {label}
          </AppText>
        </View>
        <Icon name="next" tone="subtle-foreground" />
      </Pressable>
    );
  }

  return (
    <View style={cardStyle} className={`${bannerClassName} bg-surface`}>
      <View className="h-10 w-10 items-center justify-center rounded-xl bg-muted">
        <Icon name="dayDone" tone="muted-foreground" />
      </View>
      <View className="min-w-0 flex-1">
        <AppText variant="overline" tone="subtle">
          {translate("home.banner.next")}
        </AppText>
        <AppText variant="bodyStrong" numberOfLines={1}>
          {translate("home.banner.noneLeft")}
        </AppText>
      </View>
    </View>
  );
}
