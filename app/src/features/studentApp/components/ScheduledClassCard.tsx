import { View } from "react-native";
import {
  canCancelMakeup,
  canCancelPackClass,
  canNotifyAbsence,
  classDetails,
} from "@/features/studentApp/studentAppSchedule";
import { StudentAppNextClass } from "@/features/studentApp/types";
import { useAbsenceNotice } from "@/features/studentApp/useAbsenceNotice";
import { useMakeupBooking } from "@/features/studentApp/useMakeupBooking";
import { usePackClassBooking } from "@/features/studentApp/usePackClassBooking";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Button } from "@/ui/Button";
import { Card } from "@/ui/Card";
import { StatusPill, StatusTone } from "@/ui/StatusPill";

interface ScheduledClassCardProps {
  studentId: string;
  scheduledClass: StudentAppNextClass;
  isToday: boolean;
}

function statusOf(scheduledClass: StudentAppNextClass, isToday: boolean): [string, StatusTone] {
  if (scheduledClass.isCancelled) {
    return [translate("student.student.cancelled"), "danger"];
  }
  if (scheduledClass.absenceNotified) {
    return [translate("student.absence.status"), "danger"];
  }
  if (scheduledClass.isMakeup) {
    return [translate("student.makeup.status"), "warning"];
  }
  if (scheduledClass.isPackBooking) {
    return [translate("student.packClasses.status"), "warning"];
  }
  return [translate(isToday ? "student.day.today" : "student.schedule.upcoming"), "primary"];
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
          <AppText variant="headline">{`${scheduledClass.startTime}–${scheduledClass.endTime}`}</AppText>
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
          label={translate(absenceNotified ? "student.absence.going" : "student.absence.notify")}
          isLoading={isPending}
          onPress={() => toggleAbsence(studentId, scheduledClass)}
        />
      ) : null}
      {canCancelMakeup(scheduledClass) ? (
        <Button
          size="medium"
          variant="secondary"
          icon="cancelled"
          label={translate("student.makeup.cancel")}
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
          label={translate("student.packClasses.cancel")}
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
