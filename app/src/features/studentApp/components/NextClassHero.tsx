import { View } from "react-native";
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
import { Button } from "@/ui/Button";
import { TonalBlock } from "@/ui/TonalBlock";

interface NextClassHeroProps {
  student: AccountStudent;
  onSeeWeek: () => void;
}

export function NextClassHero({ student, onSeeWeek }: NextClassHeroProps) {
  const { toggleAbsence, isPending } = useAbsenceNotice();
  const nextClass = nextClassOf(student);
  if (nextClass === null) {
    return (
      <TonalBlock tone="muted">
        <AppText variant="badge" tone="muted">
          {translate("student.nextClass.title")}
        </AppText>
        <AppText variant="body" tone="muted">
          {translate("student.student.noClasses")}
        </AppText>
      </TonalBlock>
    );
  }
  const { absenceNotified } = nextClass;
  const countdown = absenceNotified
    ? translate("student.absence.notified")
    : countdownLabel(nextClass);
  const absenceLabel = translate(
    absenceNotified ? "student.absence.undo" : "student.absence.notGoing",
  );
  return (
    <TonalBlock tone="primary">
      <View className="flex-row items-center justify-between gap-2">
        <AppText variant="badge" tone="primarySoft">
          {translate("student.nextClass.title")}
        </AppText>
        {countdown === null ? null : (
          <View
            className={`rounded-full px-2.5 py-[3px] ${absenceNotified ? "bg-danger-soft" : "bg-surface"}`}
          >
            <AppText variant="badge" tone={absenceNotified ? "dangerSoft" : "primarySoft"}>
              {countdown}
            </AppText>
          </View>
        )}
      </View>
      <View className="gap-0.5">
        <AppText variant="hero" className={absenceNotified ? "line-through opacity-50" : ""}>
          {`${relativeDayLabel(nextClass.date)} · ${nextClass.startTime}`}
        </AppText>
        <AppText variant="body" tone="muted">
          {classDetails(nextClass)}
        </AppText>
      </View>
      <View className="flex-row gap-2">
        <View className="flex-1">
          <Button
            size="medium"
            icon="classes"
            label={translate("student.nextClass.seeWeek")}
            onPress={onSeeWeek}
          />
        </View>
        {canNotifyAbsence(nextClass) ? (
          <View className="flex-1">
            <Button
              size="medium"
              variant="secondary"
              icon={absenceNotified ? "refund" : "cancelled"}
              label={absenceLabel}
              isLoading={isPending}
              onPress={() => toggleAbsence(student.id, nextClass)}
            />
          </View>
        ) : null}
      </View>
    </TonalBlock>
  );
}
