import { View } from "react-native";
import { FamilyAttendance } from "@/features/family/types";
import { parseIsoDate } from "@/features/sessions/dates";
import { translate, translateCount, TranslationKey } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Card } from "@/ui/Card";
import { Icon } from "@/ui/Icon";

const visibleWeeks = 6;

function monthOf(isoDate: string): string {
  return translate(`months.${parseIsoDate(isoDate).getMonth() + 1}` as TranslationKey);
}

export function StreakTile({ attendance }: { attendance: FamilyAttendance }) {
  const lastWeeks = attendance.recentWeeks.slice(-visibleWeeks);
  return (
    <Card className="flex-1 gap-2 p-3.5">
      <View className="flex-row items-center gap-1.5">
        <Icon name="streak" tone="warning" />
        <AppText variant="overline" tone="subtle">
          {translate("family.streak.title")}
        </AppText>
      </View>
      <AppText variant="headline">
        {translateCount("family.streak.weeks", attendance.streakWeeks)}
      </AppText>
      <View className="flex-row gap-1">
        {Array.from({ length: visibleWeeks }, (_, index) => {
          const week = lastWeeks[index - (visibleWeeks - lastWeeks.length)];
          return (
            <View
              key={week?.weekStart ?? `week-${index}`}
              className={`h-2 flex-1 rounded ${week?.attendance === "Attended" ? "bg-warning" : "bg-muted"}`}
            />
          );
        })}
      </View>
      <AppText variant="caption" tone="subtle">
        {attendance.streakSince === null
          ? translate("family.streak.start")
          : translate("family.streak.since", { month: monthOf(attendance.streakSince) })}
      </AppText>
    </Card>
  );
}
