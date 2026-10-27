import { View } from "react-native";
import { nextStreakGoal, streakGoalWeeks } from "@/features/studentApp/studentAppAchievements";
import { translate, translateCount } from "@/i18n/translate";
import { flameColors } from "@/theme/achievementColors";
import { AppText } from "@/ui/AppText";

export function StreakGoals({ streakWeeks }: { streakWeeks: number }) {
  const nextGoal = nextStreakGoal(streakWeeks);
  return (
    <View className="gap-3">
      <View className="flex-row justify-between">
        {streakGoalWeeks.map((goalWeeks) => {
          const isReached = streakWeeks >= goalWeeks;
          return (
            <View key={goalWeeks} className="items-center gap-1.5">
              <View
                className={`h-8 w-8 rounded-full ${isReached ? "" : "border-2 border-dashed border-border-strong"}`}
                style={isReached ? { backgroundColor: flameColors.outer } : null}
              />
              <AppText variant="badge" tone={isReached ? "default" : "muted"}>
                {translate("student.streak.goalWeeks", { count: goalWeeks })}
              </AppText>
            </View>
          );
        })}
      </View>
      <AppText variant="bodyStrong">
        {nextGoal === null
          ? translate("student.streak.allGoalsReached")
          : translateCount("student.streak.nextGoal", nextGoal)}
      </AppText>
    </View>
  );
}
