import { Pressable, View } from "react-native";
import {
  classDetails,
  countdownLabel,
  nextClassOf,
  relativeDayLabel,
} from "@/features/family/familySchedule";
import { FamilyStudent } from "@/features/family/types";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Card } from "@/ui/Card";
import { Icon } from "@/ui/Icon";
import { useElevationStyle } from "@/ui/elevation";

interface NextClassHeroProps {
  student: FamilyStudent;
  onSeeWeek: () => void;
}

export function NextClassHero({ student, onSeeWeek }: NextClassHeroProps) {
  const floatingStyle = useElevationStyle("floating");
  const nextClass = nextClassOf(student);
  if (nextClass === null) {
    return (
      <Card className="gap-2 p-[18px]">
        <AppText variant="overline" tone="subtle">
          {translate("family.nextClass.title")}
        </AppText>
        <AppText variant="body" tone="muted">
          {translate("family.student.noClasses")}
        </AppText>
      </Card>
    );
  }
  const countdown = countdownLabel(nextClass);
  return (
    <View style={floatingStyle} className="gap-3.5 rounded-3xl bg-primary p-[18px]">
      <View className="flex-row items-center justify-between gap-2">
        <AppText variant="overline" tone="onPrimary" className="opacity-85">
          {translate("family.nextClass.title")}
        </AppText>
        {countdown === null ? null : (
          <View className="rounded-full bg-primary-foreground/20 px-2.5 py-[5px]">
            <AppText variant="badge" tone="onPrimary">
              {countdown}
            </AppText>
          </View>
        )}
      </View>
      <View className="gap-1">
        <AppText variant="hero" tone="onPrimary">
          {`${relativeDayLabel(nextClass.date)} · ${nextClass.startTime}`}
        </AppText>
        <AppText variant="body" tone="onPrimary" className="opacity-90">
          {classDetails(nextClass)}
        </AppText>
      </View>
      <Pressable
        accessibilityRole="button"
        onPress={onSeeWeek}
        className="h-11 flex-row items-center justify-center gap-1.5 rounded-[14px] bg-surface active:opacity-80"
      >
        <Icon name="classes" size="medium" tone="primary-strong" />
        <AppText variant="bodyStrong" tone="primary">
          {translate("family.nextClass.seeWeek")}
        </AppText>
      </Pressable>
    </View>
  );
}
