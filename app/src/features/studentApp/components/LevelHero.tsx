import { View } from "react-native";
import { LevelProgress } from "@/features/studentApp/studentAppAchievements";
import { translate, translateCount } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Card } from "@/ui/Card";

interface LevelHeroProps {
  level: number;
  progress: LevelProgress;
}

export function LevelHero({ level, progress }: LevelHeroProps) {
  return (
    <Card className="gap-3 p-4">
      <View className="flex-row items-center gap-3.5">
        <View className="h-14 w-14 items-center justify-center rounded-full border-4 border-primary bg-primary-soft">
          <AppText variant="headline" tone="primary">
            {level}
          </AppText>
        </View>
        <View className="min-w-0 flex-1 gap-0.5">
          <AppText variant="overline" tone="primary">
            {translate("student.progress.level", {
              number: level,
              name: progress.current?.name ?? "",
            })}
          </AppText>
          <AppText variant="bodyStrong">
            {progress.next === null
              ? translate("student.progress.maxLevel")
              : translateCount("student.progress.classesToNext", progress.classesToNext, {
                  name: progress.next.name,
                })}
          </AppText>
        </View>
      </View>
      <View
        className="h-2.5 overflow-hidden rounded-full bg-muted"
        accessibilityRole="progressbar"
        accessibilityValue={{ min: 0, max: 100, now: progress.percent }}
      >
        <View
          className="h-full rounded-full bg-primary"
          style={{ width: `${progress.percent}%` }}
        />
      </View>
    </Card>
  );
}
