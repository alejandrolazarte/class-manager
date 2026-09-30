import { Pressable, View } from "react-native";
import { DaySession } from "@/features/sessions/types";
import { translate, translateCount } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Card } from "@/ui/Card";
import { Icon } from "@/ui/Icon";
import { useElevationStyle } from "@/ui/elevation";

interface NextSessionHeroProps {
  session: DaySession | null;
  isNow: boolean;
  onOpen: () => void;
}

const detailSeparator = " · ";

export function NextSessionHero({ session, isNow, onOpen }: NextSessionHeroProps) {
  const floatingStyle = useElevationStyle("floating");
  if (session === null) {
    return (
      <Card className="gap-2 p-[18px]">
        <AppText variant="overline" tone="subtle">
          {translate("home.nextSession.title")}
        </AppText>
        <AppText variant="body" tone="muted">
          {translate("home.nextSession.noneLeft")}
        </AppText>
      </Card>
    );
  }
  const details = [session.classGroupName, session.location].filter(Boolean).join(detailSeparator);
  return (
    <View style={floatingStyle} className="gap-3.5 rounded-3xl bg-primary p-[18px]">
      <View className="flex-row items-center justify-between gap-2">
        <AppText variant="overline" tone="onPrimary" className="opacity-85">
          {translate("home.nextSession.title")}
        </AppText>
        <View className="rounded-full bg-primary-foreground/20 px-2.5 py-[5px]">
          <AppText variant="badge" tone="onPrimary">
            {translate(isNow ? "home.nextSession.now" : "home.nextSession.today")}
          </AppText>
        </View>
      </View>
      <View className="gap-1">
        <AppText variant="hero" tone="onPrimary">
          {`${session.startTime}–${session.endTime}`}
        </AppText>
        <AppText variant="body" tone="onPrimary" className="opacity-90">
          {details}
        </AppText>
        <AppText variant="bodyStrong" tone="onPrimary" className="opacity-90">
          {translateCount("home.nextSession.students", session.enrolledCount)}
        </AppText>
      </View>
      <Pressable
        accessibilityRole="button"
        accessibilityLabel={translate("home.nextSession.takeAttendance")}
        onPress={onOpen}
        className="h-11 flex-row items-center justify-center gap-1.5 rounded-[14px] bg-surface active:opacity-80"
      >
        <Icon name="allPresent" size="medium" tone="primary-strong" />
        <AppText variant="bodyStrong" tone="primary">
          {translate("home.nextSession.takeAttendance")}
        </AppText>
      </Pressable>
    </View>
  );
}
