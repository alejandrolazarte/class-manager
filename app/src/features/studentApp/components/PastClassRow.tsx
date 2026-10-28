import { View } from "react-native";
import { pastClassStatusOf } from "@/features/studentApp/studentAppAgenda";
import { classDetails, dayAndMonthLabel } from "@/features/studentApp/studentAppSchedule";
import { StudentAppPastClass } from "@/features/studentApp/types";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { HairlineRow } from "@/ui/HairlineRow";
import { StatusPill } from "@/ui/StatusPill";

interface PastClassRowProps {
  pastClass: StudentAppPastClass;
}

const dimmedStatuses = new Set<StudentAppPastClass["status"]>(["Cancelled", "AbsenceNotified"]);

export function PastClassRow({ pastClass }: PastClassRowProps) {
  const [statusLabel, statusTone] = pastClassStatusOf(pastClass);
  const isDimmed = dimmedStatuses.has(pastClass.status);
  return (
    <HairlineRow isDimmed={isDimmed}>
      <View className="min-w-0 flex-1 gap-0.5">
        <AppText
          variant="heading"
          tone={isDimmed ? "muted" : "default"}
          className={isDimmed ? "line-through" : ""}
        >{`${pastClass.startTime}–${pastClass.endTime}`}</AppText>
        <AppText variant="caption" tone="muted">
          {classDetails(pastClass)}
        </AppText>
        {pastClass.paidWithPack === null ? null : (
          <AppText variant="caption" tone="subtle">
            {translate("student.history.paidWithPack", {
              pack: pastClass.paidWithPack.name,
              date: dayAndMonthLabel(pastClass.paidWithPack.purchasedOn),
            })}
          </AppText>
        )}
      </View>
      <StatusPill isSmall label={statusLabel} tone={statusTone} />
    </HairlineRow>
  );
}
