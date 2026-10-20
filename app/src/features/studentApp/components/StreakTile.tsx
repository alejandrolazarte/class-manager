import { View } from "react-native";
import { StudentAppAttendance } from "@/features/studentApp/types";
import { parseIsoDate } from "@/dates/dates";
import { translate, translateCount, TranslationKey } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";

const visibleWeeks = 6;

function monthOf(isoDate: string): string {
  return translate(`months.${parseIsoDate(isoDate).getMonth() + 1}` as TranslationKey);
}

export function StreakTile({ attendance }: { attendance: StudentAppAttendance }) {
  const lastWeeks = attendance.recentWeeks.slice(-visibleWeeks);
  return (
    <>
      <AppText variant="badge" tone="muted">
        {translate("student.streak.title")}
      </AppText>
      <AppText variant="headline">
        {translateCount("student.streak.weeks", attendance.streakWeeks)}
      </AppText>
      <View className="mt-1 flex-row gap-1">
        {Array.from({ length: visibleWeeks }, (_, index) => {
          const week = lastWeeks[index - (visibleWeeks - lastWeeks.length)];
          return (
            <View
              key={week?.weekStart ?? `week-${index}`}
              className={`h-[3px] flex-1 rounded-full ${week?.attendance === "Attended" ? "bg-warning" : "bg-muted"}`}
            />
          );
        })}
      </View>
      <AppText variant="footnote" tone="muted">
        {attendance.streakSince === null
          ? translate("student.streak.start")
          : translate("student.streak.since", { month: monthOf(attendance.streakSince) })}
      </AppText>
    </>
  );
}
