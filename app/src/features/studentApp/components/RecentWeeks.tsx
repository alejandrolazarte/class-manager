import { View } from "react-native";
import { StudentAppAttendance } from "@/features/studentApp/types";
import { translate, translateCount } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";

const weekClassNames = {
  Attended: "bg-success",
  Missed: "bg-danger-soft",
  NoClasses: "bg-muted",
} as const;

export function RecentWeeks({ attendance }: { attendance: StudentAppAttendance }) {
  return (
    <View className="gap-2">
      <View className="flex-row gap-[5px]">
        {attendance.recentWeeks.map((week) => (
          <View
            key={week.weekStart}
            className={`h-7 flex-1 rounded-[5px] ${weekClassNames[week.attendance] ?? "bg-muted"}`}
          />
        ))}
      </View>
      <AppText variant="caption" tone="muted">
        {attendance.bestStreakWeeks > 0
          ? translateCount("student.progress.bestStreak", attendance.bestStreakWeeks)
          : translate("student.streak.start")}
      </AppText>
    </View>
  );
}
