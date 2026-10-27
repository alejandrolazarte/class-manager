import { View } from "react-native";
import { StudentAppAttendance, WeekAttendance } from "@/features/studentApp/types";
import { translate, translateCount } from "@/i18n/translate";
import { flameColors } from "@/theme/achievementColors";
import { AppText } from "@/ui/AppText";
import { StreakFlame } from "@/ui/StreakFlame";
import { TextLink } from "@/ui/TextLink";
import { TonalBlock } from "@/ui/TonalBlock";

interface StreakCardProps {
  attendance: StudentAppAttendance;
  onSeeStreak: () => void;
}

const shownWeeks = 8;

const weekClassNames: Record<WeekAttendance, string> = {
  Attended: "",
  Missed: "bg-inverse-foreground/40",
  NoClasses: "bg-inverse-foreground/15",
};

export function StreakCard({ attendance, onSeeStreak }: StreakCardProps) {
  const recentWeeks = attendance.recentWeeks.slice(-shownWeeks);
  return (
    <TonalBlock tone="inverse">
      <View className="flex-row items-center gap-3.5">
        <StreakFlame isLit={attendance.streakWeeks > 0} size="medium" />
        <View className="min-w-0 flex-1 gap-0.5">
          <AppText variant="headline" tone="inverse">
            {attendance.streakWeeks > 0
              ? translateCount("student.progress.streakWeeks", attendance.streakWeeks)
              : translate("student.streak.start")}
          </AppText>
          {attendance.bestStreakWeeks > 0 ? (
            <AppText variant="caption" tone="inverse" className="opacity-70">
              {translateCount("student.progress.bestStreak", attendance.bestStreakWeeks)}
            </AppText>
          ) : null}
        </View>
      </View>
      {recentWeeks.length > 0 ? (
        <View className="flex-row gap-1">
          {recentWeeks.map((week) => (
            <View
              key={week.weekStart}
              className={`h-2 flex-1 rounded-full ${weekClassNames[week.attendance]}`}
              style={week.attendance === "Attended" ? { backgroundColor: flameColors.outer } : null}
            />
          ))}
        </View>
      ) : null}
      <View className="border-t border-inverse-foreground/15 pt-1">
        <TextLink
          label={translate("student.progress.seeStreak")}
          trailingIcon="next"
          onPress={onSeeStreak}
          isOnInverse
        />
      </View>
    </TonalBlock>
  );
}
