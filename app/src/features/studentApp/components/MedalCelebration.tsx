import { Modal, View } from "react-native";
import { medalIcons, medalMetals } from "@/features/studentApp/studentAppAchievements";
import { Medal } from "@/features/studentApp/types";
import { translate, TranslationKey } from "@/i18n/translate";
import { useCloseOnBack } from "@/navigation/useCloseOnBack";
import { ThemeScope } from "@/theme/ThemeScope";
import { AppText } from "@/ui/AppText";
import { Button } from "@/ui/Button";
import { MedalBadge } from "@/ui/MedalBadge";
import { Confetti } from "@/ui/motion/Confetti";
import { Entrance } from "@/ui/motion/Entrance";

interface MedalCelebrationProps {
  medal: Medal;
  nextMedal: Medal | null;
  onClose: () => void;
}

function medalName(medal: Medal): string {
  return translate(`student.medals.${medal}` as TranslationKey);
}

export function MedalCelebration({ medal, nextMedal, onClose }: MedalCelebrationProps) {
  useCloseOnBack(onClose);
  return (
    <Modal transparent visible animationType="none" onRequestClose={onClose}>
      <ThemeScope className="flex-1 bg-inverse">
        <Confetti />
        <View className="flex-1 items-center justify-center gap-6 px-6">
          <Entrance kind="pop">
            <MedalBadge icon={medalIcons[medal]} metal={medalMetals[medal]} size="hero" />
          </Entrance>
          <Entrance kind="springRise" className="items-center gap-2">
            <AppText variant="overline" tone="inverse" className="opacity-80">
              {translate("student.celebration.title")}
            </AppText>
            <AppText
              variant="hero"
              tone="inverse"
              accessibilityRole="header"
              className="text-center"
            >
              {medalName(medal)}
            </AppText>
            <AppText variant="body" tone="inverse" className="text-center opacity-80">
              {translate(`student.medals.description.${medal}` as TranslationKey)}
            </AppText>
            {nextMedal === null ? null : (
              <View className="mt-2 rounded-full bg-inverse-foreground/10 px-3.5 py-2">
                <AppText variant="caption" tone="inverse">
                  {translate("student.celebration.next", { name: medalName(nextMedal) })}
                </AppText>
              </View>
            )}
          </Entrance>
        </View>
        <View className="px-6 pb-8">
          <Button label={translate("common.continue")} onPress={onClose} />
        </View>
      </ThemeScope>
    </Modal>
  );
}
