import { View } from "react-native";
import { MedalGoal, medalIcons } from "@/features/studentApp/studentAppAchievements";
import { translate, translateCount, TranslationKey } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { MedalBadge } from "@/ui/MedalBadge";
import { TonalBlock } from "@/ui/TonalBlock";

const remainingKeys = {
  classes: "student.progress.nextMedal.classes",
  weeks: "student.progress.nextMedal.weeks",
} as const;

export function NextMedalBanner({ goal }: { goal: MedalGoal }) {
  const name = translate(`student.medals.${goal.medal}` as TranslationKey);
  return (
    <TonalBlock tone="warning" isCompact>
      <View className="flex-row items-center gap-3">
        <MedalBadge icon={medalIcons[goal.medal]} metal={null} size="small" />
        <View className="min-w-0 flex-1 gap-0.5">
          <AppText variant="overline" tone="warningSoft">
            {translate("student.progress.nextMedal.title")}
          </AppText>
          <AppText variant="bodyStrong" tone="warningSoft">
            {translateCount(remainingKeys[goal.unit], goal.total - goal.done, { name })}
          </AppText>
        </View>
      </View>
    </TonalBlock>
  );
}
