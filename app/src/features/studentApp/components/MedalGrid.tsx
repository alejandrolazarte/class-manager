import { View } from "react-native";
import { allMedals, medalIcons } from "@/features/studentApp/studentAppAchievements";
import { Medal } from "@/features/studentApp/types";
import { translate, TranslationKey } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Icon } from "@/ui/Icon";

export function MedalGrid({ earned }: { earned: readonly Medal[] }) {
  return (
    <View className="flex-row flex-wrap gap-y-3.5">
      {allMedals.map((medal) => {
        const isEarned = earned.includes(medal);
        const name = translate(`student.medals.${medal}` as TranslationKey);
        return (
          <View
            key={medal}
            accessible
            accessibilityLabel={translate(
              isEarned ? "student.progress.medalEarned" : "student.progress.medalLocked",
              { name },
            )}
            className="w-1/3 items-center gap-1.5 px-1"
          >
            <View
              className={`h-12 w-12 items-center justify-center rounded-full ${isEarned ? "bg-warning-soft" : "bg-muted"}`}
            >
              <Icon
                name={isEarned ? medalIcons[medal] : "locked"}
                tone={isEarned ? "warning" : "disabled-foreground"}
              />
            </View>
            <AppText
              variant="footnote"
              tone={isEarned ? "default" : "subtle"}
              className="text-center font-label"
            >
              {name}
            </AppText>
          </View>
        );
      })}
    </View>
  );
}
