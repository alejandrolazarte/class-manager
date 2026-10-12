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
import { HairlineRow } from "@/ui/HairlineRow";
import { Icon } from "@/ui/Icon";
import { StatusPill, StatusTone } from "@/ui/StatusPill";

interface ScheduledClassRowProps {
  studentId: string;
  scheduledClass: StudentAppNextClass;
  isToday: boolean;
  onOpen?: () => void;
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

export function ScheduledClassRow({
  studentId,
  scheduledClass,
  isToday,
  onOpen,
}: ScheduledClassRowProps) {
  const { toggleAbsence, isPending } = useAbsenceNotice();
  const { toggleMakeup, pendingKey } = useMakeupBooking();
  const packClassBooking = usePackClassBooking();
  const { isCancelled, absenceNotified } = scheduledClass;
  const [statusLabel, statusTone] = statusOf(scheduledClass, isToday);
  const isDimmed = isCancelled || absenceNotified;
  const summary = (
    <>
      <View className="min-w-0 flex-1 gap-0.5">
        <AppText
          variant="heading"
          tone={isDimmed ? "muted" : "default"}
          className={isDimmed ? "line-through" : ""}
        >{`${scheduledClass.startTime}–${scheduledClass.endTime}`}</AppText>
        <AppText variant="caption" tone="muted">
          {classDetails(scheduledClass)}
        </AppText>
      </View>
      <StatusPill isSmall label={statusLabel} tone={statusTone} />
    </>
  );
  if (onOpen !== undefined) {
    return (
      <HairlineRow
        isDimmed={isDimmed}
        accessibilityLabel={translate("student.classDetail.open", {
          className: scheduledClass.name,
        })}
        onPress={onOpen}
      >
        {summary}
        <Icon name="next" size="small" tone="subtle-foreground" />
      </HairlineRow>
    );
  }
  return (
    <HairlineRow isDimmed={isDimmed} className="gap-2.5 py-3">
      <View className="flex-row items-start gap-3">{summary}</View>
      {canNotifyAbsence(scheduledClass) ? (
        <View className="self-start">
          <Button
            size="medium"
            variant="secondary"
            icon={absenceNotified ? "refund" : "cancelled"}
            label={translate(absenceNotified ? "student.absence.going" : "student.absence.notify")}
            isLoading={isPending}
            onPress={() => toggleAbsence(studentId, scheduledClass)}
          />
        </View>
      ) : null}
      {canCancelMakeup(scheduledClass) ? (
        <View className="self-start">
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
        </View>
      ) : null}
      {canCancelPackClass(scheduledClass) ? (
        <View className="self-start">
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
        </View>
      ) : null}
    </HairlineRow>
  );
}
