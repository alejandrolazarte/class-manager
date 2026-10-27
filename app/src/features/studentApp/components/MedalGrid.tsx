import { View } from "react-native";
import {
  allMedals,
  medalGoal,
  medalIcons,
  medalMetals,
} from "@/features/studentApp/studentAppAchievements";
import { StudentAppAttendance } from "@/features/studentApp/types";
import { translate, TranslationKey } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { MedalBadge } from "@/ui/MedalBadge";
import { ProgressBar } from "@/ui/ProgressBar";

export function MedalGrid({ attendance }: { attendance: StudentAppAttendance }) {
  return (
    <View className="flex-row flex-wrap justify-between gap-y-2.5">
      {allMedals.map((medal) => {
        const isEarned = attendance.medals.includes(medal);
        const name = translate(`student.medals.${medal}` as TranslationKey);
        const goal = isEarned ? null : medalGoal(medal, attendance);
        return (
          <View
            key={medal}
            accessible
            accessibilityLabel={translate(
              isEarned ? "student.progress.medalEarned" : "student.progress.medalLocked",
              { name },
            )}
            className="w-[31.5%] items-center gap-1.5 rounded-2xl bg-surface px-1.5 pb-3 pt-2.5"
          >
            <MedalBadge icon={medalIcons[medal]} metal={isEarned ? medalMetals[medal] : null} />
            <AppText
              variant="footnote"
              tone={isEarned ? "default" : "muted"}
              className="text-center font-label"
            >
              {name}
            </AppText>
            {isEarned ? (
              <AppText variant="overline" tone="warning">
                {translate(`student.medals.metal.${medalMetals[medal]}` as TranslationKey)}
              </AppText>
            ) : goal === null ? null : (
              <View className="w-3/4 items-center gap-1">
                <View className="w-full flex-row">
                  <ProgressBar ratio={goal.done / goal.total} />
                </View>
                <AppText variant="pill" tone="primary">
                  {translate("student.progress.medalProgress", {
                    count: goal.done,
                    total: goal.total,
                  })}
                </AppText>
              </View>
            )}
          </View>
        );
      })}
    </View>
  );
}
