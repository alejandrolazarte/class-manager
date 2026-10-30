import { View } from "react-native";
import {
  allMedals,
  medalGoal,
  medalIcons,
  medalMetals,
} from "@/features/studentApp/studentAppAchievements";
import { Medal, StudentAppAttendance } from "@/features/studentApp/types";
import { translate, TranslationKey } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { MedalBadge } from "@/ui/MedalBadge";
import { PressableScale } from "@/ui/motion/PressableScale";
import { ProgressBar } from "@/ui/ProgressBar";

interface MedalGridProps {
  attendance: StudentAppAttendance;
  onOpenEarnedMedal: (medal: Medal) => void;
}

const medalCellClassName =
  "w-[31.5%] items-center gap-1.5 rounded-2xl bg-surface px-1.5 pb-3 pt-2.5";

export function MedalGrid({ attendance, onOpenEarnedMedal }: MedalGridProps) {
  return (
    <View className="flex-row flex-wrap justify-between gap-y-2.5">
      {allMedals.map((medal) => {
        const isEarned = attendance.medals.includes(medal);
        const name = translate(`student.medals.${medal}` as TranslationKey);
        const goal = isEarned ? null : medalGoal(medal, attendance);
        const content = (
          <>
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
          </>
        );
        if (isEarned) {
          return (
            <PressableScale
              key={medal}
              accessibilityRole="button"
              accessibilityLabel={translate("student.progress.medalEarned", { name })}
              onPress={() => onOpenEarnedMedal(medal)}
              className={`${medalCellClassName} active:bg-muted`}
            >
              {content}
            </PressableScale>
          );
        }
        return (
          <View
            key={medal}
            accessible
            accessibilityLabel={translate("student.progress.medalLocked", { name })}
            className={medalCellClassName}
          >
            {content}
          </View>
        );
      })}
    </View>
  );
}
