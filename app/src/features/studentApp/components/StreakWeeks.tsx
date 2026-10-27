import { View } from "react-native";
import { StudentAppAttendanceWeek, WeekAttendance } from "@/features/studentApp/types";
import { translate, TranslationKey } from "@/i18n/translate";
import { flameColors } from "@/theme/achievementColors";
import { AppText } from "@/ui/AppText";
import { StreakFlame } from "@/ui/StreakFlame";

const legendKeys: Record<WeekAttendance, TranslationKey> = {
  Attended: "student.streak.legend.attended",
  Missed: "student.streak.legend.missed",
  NoClasses: "student.streak.legend.noClasses",
};

const legendDotClassNames: Record<WeekAttendance, string> = {
  Attended: "",
  Missed: "bg-border-strong",
  NoClasses: "bg-border",
};

const legendOrder: readonly WeekAttendance[] = ["Attended", "Missed", "NoClasses"];

function WeekMark({ attendance }: { attendance: WeekAttendance }) {
  if (attendance === "NoClasses") {
    return <View className="h-2 w-2 rounded-full bg-border" />;
  }
  return <StreakFlame isLit={attendance === "Attended"} size="small" />;
}

export function StreakWeeks({ weeks }: { weeks: readonly StudentAppAttendanceWeek[] }) {
  return (
    <View className="gap-3">
      <View className="h-7 flex-row items-end justify-between">
        {weeks.map((week) => (
          <View
            key={week.weekStart}
            accessible
            accessibilityLabel={translate(legendKeys[week.attendance])}
            className="flex-1 items-center"
          >
            <WeekMark attendance={week.attendance} />
          </View>
        ))}
      </View>
      <View className="flex-row flex-wrap gap-x-4 gap-y-1">
        {legendOrder.map((attendance) => (
          <View key={attendance} className="flex-row items-center gap-1.5">
            <View
              className={`h-2.5 w-2.5 rounded-full ${legendDotClassNames[attendance]}`}
              style={attendance === "Attended" ? { backgroundColor: flameColors.outer } : null}
            />
            <AppText variant="footnote" tone="muted">
              {translate(legendKeys[attendance])}
            </AppText>
          </View>
        ))}
      </View>
    </View>
  );
}
