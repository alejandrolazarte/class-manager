import { View } from "react-native";
import { classDetails } from "@/features/family/familySchedule";
import { FamilyNextClass } from "@/features/family/types";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Card } from "@/ui/Card";
import { StatusPill } from "@/ui/StatusPill";

interface ScheduledClassCardProps {
  scheduledClass: FamilyNextClass;
  isToday: boolean;
}

export function ScheduledClassCard({ scheduledClass, isToday }: ScheduledClassCardProps) {
  const { isCancelled } = scheduledClass;
  return (
    <Card className={`flex-row items-center gap-3 px-4 py-3.5 ${isCancelled ? "opacity-70" : ""}`}>
      <View className={`w-1 self-stretch rounded-sm ${isCancelled ? "bg-danger" : "bg-primary"}`} />
      <View className="min-w-0 flex-1 gap-0.5">
        <AppText variant="title">{`${scheduledClass.startTime}–${scheduledClass.endTime}`}</AppText>
        <AppText variant="body" tone="muted">
          {classDetails(scheduledClass)}
        </AppText>
      </View>
      <StatusPill
        label={translate(
          isCancelled
            ? "family.student.cancelled"
            : isToday
              ? "family.day.today"
              : "family.schedule.upcoming",
        )}
        tone={isCancelled ? "danger" : "primary"}
      />
    </Card>
  );
}
