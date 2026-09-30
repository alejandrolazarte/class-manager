import { View } from "react-native";
import { FamilyAttendance } from "@/features/family/types";
import { translate, translateCount } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Card } from "@/ui/Card";

const weekClassNames = {
  Attended: "bg-success",
  Missed: "bg-danger-soft",
  NoClasses: "bg-muted",
} as const;

export function WeeksCard({ attendance }: { attendance: FamilyAttendance }) {
  return (
    <Card className="gap-3 p-4">
      <View className="flex-row gap-1.5">
        {attendance.recentWeeks.map((week) => (
          <View
            key={week.weekStart}
            className={`h-7 flex-1 rounded-md ${weekClassNames[week.attendance] ?? "bg-muted"}`}
          />
        ))}
      </View>
      <AppText variant="caption" tone="muted">
        {attendance.bestStreakWeeks > 0
          ? translateCount("family.progress.bestStreak", attendance.bestStreakWeeks)
          : translate("family.streak.start")}
      </AppText>
    </Card>
  );
}
