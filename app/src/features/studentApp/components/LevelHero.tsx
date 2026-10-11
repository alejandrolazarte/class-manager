import { View } from "react-native";
import { LevelProgress } from "@/features/studentApp/studentAppAchievements";
import { translate, translateCount } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { ProgressBar } from "@/ui/ProgressBar";

interface LevelHeroProps {
  level: number;
  progress: LevelProgress;
}

const fullPercentage = 100;

export function LevelHero({ level, progress }: LevelHeroProps) {
  return (
    <View className="gap-3">
      <View className="flex-row items-center gap-3.5">
        <View className="h-[60px] w-[60px] items-center justify-center rounded-full border-4 border-primary bg-primary-soft">
          <AppText variant="headline" tone="primarySoft">
            {level}
          </AppText>
        </View>
        <View className="min-w-0 flex-1 gap-0.5">
          <AppText variant="badge" tone="primary">
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
      <View className="flex-row">
        <ProgressBar ratio={progress.percent / fullPercentage} isThin />
      </View>
    </View>
  );
}
