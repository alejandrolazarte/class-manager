import { Pressable, View } from "react-native";
import {
  canNotifyAbsence,
  classDetails,
  countdownLabel,
  nextClassOf,
  relativeDayLabel,
} from "@/features/studentApp/studentAppSchedule";
import { AccountStudent } from "@/features/studentApp/types";
import { useAbsenceNotice } from "@/features/studentApp/useAbsenceNotice";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Card } from "@/ui/Card";
import { Icon } from "@/ui/Icon";
import { useElevationStyle } from "@/ui/elevation";

interface NextClassHeroProps {
  student: AccountStudent;
  onSeeWeek: () => void;
}

export function NextClassHero({ student, onSeeWeek }: NextClassHeroProps) {
  const floatingStyle = useElevationStyle("floating");
  const { toggleAbsence, isPending } = useAbsenceNotice();
  const nextClass = nextClassOf(student);
  if (nextClass === null) {
    return (
      <Card className="gap-2 p-[18px]">
        <AppText variant="overline" tone="subtle">
          {translate("student.nextClass.title")}
        </AppText>
        <AppText variant="body" tone="muted">
          {translate("student.student.noClasses")}
        </AppText>
      </Card>
    );
  }
  const countdown = nextClass.absenceNotified
    ? translate("student.absence.notified")
    : countdownLabel(nextClass);
  return (
    <View style={floatingStyle} className="gap-3.5 rounded-3xl bg-primary p-[18px]">
      <View className="flex-row items-center justify-between gap-2">
        <AppText variant="overline" tone="onPrimary" className="opacity-85">
          {translate("student.nextClass.title")}
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
      <View className="flex-row gap-2">
        <Pressable
          accessibilityRole="button"
          accessibilityLabel={translate("student.nextClass.seeWeek")}
          onPress={onSeeWeek}
          className="h-11 flex-1 flex-row items-center justify-center gap-1.5 rounded-[14px] bg-surface active:opacity-80"
        >
          <Icon name="classes" size="medium" tone="primary-strong" />
          <AppText variant="bodyStrong" tone="primary">
            {translate("student.nextClass.seeWeek")}
          </AppText>
        </Pressable>
        {canNotifyAbsence(nextClass) ? (
          <Pressable
            accessibilityRole="button"
            accessibilityLabel={translate(
              nextClass.absenceNotified ? "student.absence.undo" : "student.absence.notGoing",
            )}
            accessibilityState={{ busy: isPending }}
            disabled={isPending}
            onPress={() => toggleAbsence(student.id, nextClass)}
            className="h-11 flex-1 flex-row items-center justify-center gap-1.5 rounded-[14px] border-[1.5px] border-primary-foreground/50 active:opacity-80"
          >
            <Icon
              name={nextClass.absenceNotified ? "refund" : "cancelled"}
              size="medium"
              tone="primary-foreground"
            />
            <AppText variant="bodyStrong" tone="onPrimary">
              {translate(
                nextClass.absenceNotified ? "student.absence.undo" : "student.absence.notGoing",
              )}
            </AppText>
          </Pressable>
        ) : null}
      </View>
    </View>
  );
}
