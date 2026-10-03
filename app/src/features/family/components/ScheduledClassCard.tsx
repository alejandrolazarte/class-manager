import { View } from "react-native";
import {
  canCancelMakeup,
  canCancelPackClass,
  canNotifyAbsence,
  classDetails,
} from "@/features/family/familySchedule";
import { FamilyNextClass } from "@/features/family/types";
import { useAbsenceNotice } from "@/features/family/useAbsenceNotice";
import { useMakeupBooking } from "@/features/family/useMakeupBooking";
import { usePackClassBooking } from "@/features/family/usePackClassBooking";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Button } from "@/ui/Button";
import { Card } from "@/ui/Card";
import { StatusPill, StatusTone } from "@/ui/StatusPill";

interface ScheduledClassCardProps {
  studentId: string;
  scheduledClass: FamilyNextClass;
  isToday: boolean;
}

function statusOf(scheduledClass: FamilyNextClass, isToday: boolean): [string, StatusTone] {
  if (scheduledClass.isCancelled) {
    return [translate("family.student.cancelled"), "danger"];
  }
  if (scheduledClass.absenceNotified) {
    return [translate("family.absence.status"), "danger"];
  }
  if (scheduledClass.isMakeup) {
    return [translate("family.makeup.status"), "warning"];
  }
  if (scheduledClass.isPackBooking) {
    return [translate("family.packClasses.status"), "warning"];
  }
  return [translate(isToday ? "family.day.today" : "family.schedule.upcoming"), "primary"];
}

export function ScheduledClassCard({
  studentId,
  scheduledClass,
  isToday,
}: ScheduledClassCardProps) {
  const { toggleAbsence, isPending } = useAbsenceNotice();
  const { toggleMakeup, pendingKey } = useMakeupBooking();
  const packClassBooking = usePackClassBooking();
  const isBooked = scheduledClass.isMakeup || scheduledClass.isPackBooking;
  const { isCancelled, absenceNotified } = scheduledClass;
  const [statusLabel, statusTone] = statusOf(scheduledClass, isToday);
  const isDimmed = isCancelled || absenceNotified;
  return (
    <Card className={`gap-3 px-4 py-3.5 ${isDimmed ? "opacity-80" : ""}`}>
      <View className="flex-row items-center gap-3">
        <View
          className={`w-1 self-stretch rounded-sm ${isDimmed ? "bg-danger" : isBooked ? "bg-warning" : "bg-primary"}`}
        />
        <View className="min-w-0 flex-1 gap-0.5">
          <AppText variant="title">{`${scheduledClass.startTime}–${scheduledClass.endTime}`}</AppText>
          <AppText variant="body" tone="muted">
            {classDetails(scheduledClass)}
          </AppText>
        </View>
        <StatusPill label={statusLabel} tone={statusTone} />
      </View>
      {canNotifyAbsence(scheduledClass) ? (
        <Button
          size="medium"
          variant="secondary"
          icon={absenceNotified ? "refund" : "cancelled"}
          label={translate(absenceNotified ? "family.absence.going" : "family.absence.notify")}
          isLoading={isPending}
          onPress={() => toggleAbsence(studentId, scheduledClass)}
        />
      ) : null}
      {canCancelMakeup(scheduledClass) ? (
        <Button
          size="medium"
          variant="secondary"
          icon="cancelled"
          label={translate("family.makeup.cancel")}
          isLoading={pendingKey !== null}
          onPress={() =>
            toggleMakeup({
              studentId,
              classGroupId: scheduledClass.classGroupId ?? "",
              date: scheduledClass.date,
              isBooked: true,
            })
          }
        />
      ) : null}
      {canCancelPackClass(scheduledClass) ? (
        <Button
          size="medium"
          variant="secondary"
          icon="cancelled"
          label={translate("family.packClasses.cancel")}
          isLoading={packClassBooking.pendingKey !== null}
          onPress={() =>
            packClassBooking.togglePackClass({
              studentId,
              classGroupId: scheduledClass.classGroupId ?? "",
              date: scheduledClass.date,
              isBooked: true,
            })
          }
        />
      ) : null}
    </Card>
  );
}
