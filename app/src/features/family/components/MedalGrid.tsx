import { View } from "react-native";
import { allMedals, medalIcons } from "@/features/family/familyAchievements";
import { Medal } from "@/features/family/types";
import { translate, TranslationKey } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Card } from "@/ui/Card";
import { Icon } from "@/ui/Icon";

export function MedalGrid({ earned }: { earned: readonly Medal[] }) {
  return (
    <View className="flex-row flex-wrap gap-3">
      {allMedals.map((medal) => {
        const isEarned = earned.includes(medal);
        const name = translate(`family.medals.${medal}` as TranslationKey);
        return (
          <Card
            key={medal}
            className={`basis-[30%] grow px-2 py-3.5 ${isEarned ? "" : "opacity-60"}`}
          >
            <View
              accessible
              accessibilityLabel={translate(
                isEarned ? "family.progress.medalEarned" : "family.progress.medalLocked",
                { name },
              )}
              className="items-center gap-2"
            >
              <View
                className={`h-12 w-12 items-center justify-center rounded-full ${isEarned ? "bg-warning-soft" : "bg-muted"}`}
              >
                <Icon
                  name={isEarned ? medalIcons[medal] : "locked"}
                  tone={isEarned ? "warning" : "subtle-foreground"}
                />
              </View>
              <AppText
                variant="caption"
                tone={isEarned ? "default" : "subtle"}
                className="text-center font-label"
              >
                {name}
              </AppText>
            </View>
          </Card>
        );
      })}
    </View>
  );
}
