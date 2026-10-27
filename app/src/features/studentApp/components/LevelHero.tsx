import { View } from "react-native";
import { LevelProgress } from "@/features/studentApp/studentAppAchievements";
import { translate, translateCount } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { ProgressRing } from "@/ui/ProgressRing";
import { TextLink } from "@/ui/TextLink";
import { TonalBlock } from "@/ui/TonalBlock";

interface LevelHeroProps {
  level: number;
  attendedClasses: number;
  progress: LevelProgress;
  onSeeAllLevels: () => void;
}

const fullPercentage = 100;

export function LevelHero({ level, attendedClasses, progress, onSeeAllLevels }: LevelHeroProps) {
  const levelLabel = translate("student.progress.level", {
    number: level,
    name: progress.current?.name ?? "",
  });
  return (
    <TonalBlock tone="surface">
      <View className="flex-row items-center gap-4">
        <ProgressRing ratio={progress.percent / fullPercentage} accessibilityLabel={levelLabel}>
          <AppText variant="hero" tone="primarySoft">
            {level}
          </AppText>
        </ProgressRing>
        <View className="min-w-0 flex-1 gap-1">
          <AppText variant="badge" tone="primary">
            {levelLabel}
          </AppText>
          <AppText variant="bodyStrong">
            {progress.next === null
              ? translate("student.progress.maxLevel")
              : translateCount("student.progress.classesToNext", progress.classesToNext, {
                  name: progress.next.name,
                })}
          </AppText>
          {progress.next === null ? null : (
            <AppText variant="footnote" tone="muted">
              {translate("student.progress.classesOfNext", {
                count: attendedClasses,
                total: progress.next.requiredClasses,
              })}
            </AppText>
          )}
        </View>
      </View>
      <View className="border-t border-border-subtle pt-1">
        <TextLink
          label={translate("student.progress.allLevels")}
          trailingIcon="next"
          onPress={onSeeAllLevels}
        />
      </View>
    </TonalBlock>
  );
}
